// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Manages the position and bindings of a list.
    /// </summary>
    public class CurrencyManager : BindingManagerBase
    {
        private object _dataSource;
        private bool _bound;
        protected int listposition = -1;

        private int _lastGoodKnownRow = -1;
        private bool _pullingData;

        private bool _inChangeRecordState;
        private bool _suspendPushDataInCurrentChanged;
        private ItemChangedEventHandler _onItemChanged;
        private ListChangedEventHandler _onListChanged;
        private readonly ItemChangedEventArgs _resetEvent = new ItemChangedEventArgs(-1);
        private EventHandler _onMetaDataChangedHandler;

        /// <summary>
        ///  Gets the type of the list.
        /// </summary>
        protected Type finalType;

        /// <summary>
        ///  Occurs when the current item has been altered.
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        public event ItemChangedEventHandler ItemChanged
        {
            add => _onItemChanged += value;
            remove => _onItemChanged -= value;
        }

        public event ListChangedEventHandler ListChanged
        {
            add => _onListChanged += value;
            remove => _onListChanged -= value;
        }

        internal CurrencyManager(object dataSource)
        {
            SetDataSource(dataSource);
        }

        /// <summary>
        ///  Gets a value indicating whether items can be added to the list.
        /// </summary>
        internal bool AllowAdd
        {
            get
            {
                if (List is IBindingList list)
                {
                    return list.AllowNew;
                }

                if (List is null)
                {
                    return false;
                }

                return !List.IsReadOnly && !List.IsFixedSize;
            }
        }

        /// <summary>
        ///  Gets a value
        ///  indicating whether edits to the list are allowed.
        /// </summary>
        internal bool AllowEdit
        {
            get
            {
                if (List is IBindingList list)
                {
                    return list.AllowEdit;
                }

                if (List is null)
                {
                    return false;
                }

                return !List.IsReadOnly;
            }
        }

        /// <summary>
        ///  Gets a value indicating whether items can be removed from the list.
        /// </summary>
        internal bool AllowRemove
        {
            get
            {
                if (List is IBindingList list)
                {
                    return list.AllowRemove;
                }

                if (List is null)
                {
                    return false;
                }

                return !List.IsReadOnly && !List.IsFixedSize;
            }
        }

        /// <summary>
        ///  Gets the number of items in the list.
        /// </summary>
        public override int Count => List is null ? 0 : List.Count;

        /// <summary>
        ///  Gets the current item in the list.
        /// </summary>
        public override object Current => this[Position];

        internal override Type BindType => ListBindingHelper.GetListItemType(List);

        /// <summary>
        ///  Gets the data source of the list.
        /// </summary>
        internal override object DataSource => _dataSource;

        private protected override void SetDataSource(object dataSource)
        {
            if (_dataSource == dataSource)
            {
                return;
            }

            Release();
            _dataSource = dataSource;
            List = null;
            finalType = null;

            object tempList = dataSource;
            if (tempList is Array array)
            {
                finalType = tempList.GetType();
                tempList = array;
            }

            if (tempList is IListSource source)
            {
                tempList = source.GetList();
            }

            if (tempList is null)
            {
                throw new ArgumentNullException(nameof(dataSource));
            }

            if (tempList is IList list)
            {
                finalType ??= tempList.GetType();

                List = list;
                WireEvents(List);

                listposition = List.Count > 0 ? 0 : -1;

                OnItemChanged(_resetEvent);
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
                UpdateIsBinding();
            }
            else
            {
                throw new ArgumentException(
                    string.Format(SR.ListManagerSetDataSource, tempList.GetType().FullName),
                    nameof(dataSource));
            }
        }

        /// <summary>
        ///  Gets a value indicating whether the list is bound to a data source.
        /// </summary>
        internal override bool IsBinding => _bound;

        // The DataGridView needs this.
        internal bool ShouldBind { get; private set; } = true;

        /// <summary>
        ///  Gets the list as an object.
        /// </summary>
        public IList List { get; private set; }

        /// <summary>
        ///  Gets or sets the position you are at within the list.
        /// </summary>
        public override int Position
        {
            get => listposition;
            set
            {
                if (listposition == -1)
                {
                    return;
                }

                if (value < 0)
                {
                    value = 0;
                }

                int count = List.Count;
                if (value >= count)
                {
                    value = count - 1;
                }

                ChangeRecordState(
                    value,
                    validating: listposition != value,
                    endCurrentEdit: true,
                    firePositionChange: true,
                    pullData: false);
            }
        }

        /// <summary>
        ///  Gets or sets the object at the specified index.
        /// </summary>
        internal object this[int index]
        {
            get
            {
                if (index < 0 || index >= List.Count)
                    throw new IndexOutOfRangeException(
                        string.Format(SR.ListManagerNoValue, index.ToString(CultureInfo.CurrentCulture)));

                return List[index];
            }
            set
            {
                if (index < 0 || index >= List.Count)
                    throw new IndexOutOfRangeException(
                        string.Format(SR.ListManagerNoValue, index.ToString(CultureInfo.CurrentCulture)));

                List[index] = value;
            }
        }

        public override void AddNew()
        {
            if (List is IBindingList ibl)
            {
                ibl.AddNew();
            }
            else
            {
                // If the list is not IBindingList, then throw an exception:
                throw new NotSupportedException(SR.CurrencyManagerCantAddNew);
            }

            ChangeRecordState(
                List.Count - 1,
                (Position != List.Count - 1),
                (Position != List.Count - 1),
                firePositionChange: true,
                pullData: true);
        }

        /// <summary>
        ///  Cancels the current edit operation.
        /// </summary>
        public override void CancelCurrentEdit()
        {
            if (Count > 0)
            {
                object item = (Position >= 0 && Position < List.Count) ? List[Position] : null;

                if (item is IEditableObject iEditableItem)
                {
                    iEditableItem.CancelEdit();
                }

                if (List is ICancelAddNew iListWithCancelAddNewSupport)
                {
                    iListWithCancelAddNewSupport.CancelNew(Position);
                }

                OnItemChanged(new ItemChangedEventArgs(Position));
                if (Position != -1)
                {
                    OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, Position));
                }
            }
        }

        private void ChangeRecordState(int newPosition, bool validating, bool endCurrentEdit, bool firePositionChange, bool pullData)
        {
            if (newPosition == -1 && List.Count == 0)
            {
                if (listposition != -1)
                {
                    listposition = -1;
                    OnPositionChanged(EventArgs.Empty);
                }
                return;
            }

            if ((newPosition < 0 || newPosition >= Count) && IsBinding)
            {
                throw new IndexOutOfRangeException(SR.ListManagerBadPosition);
            }

            // If PushData fails in the OnCurrentChanged and there was a lastGoodKnownRow then the position does not
            // change, so we should not fire the OnPositionChanged event; this is why we have to cache the old
            // position and compare that with the position that the user will want to navigate to.

            int oldPosition = listposition;
            if (endCurrentEdit)
            {
                // Do not PushData when pro.
                _inChangeRecordState = true;
                try
                {
                    EndCurrentEdit();
                }
                finally
                {
                    _inChangeRecordState = false;
                }
            }

            // We pull the data from the controls only when the ListManager changes the list. when the backEnd changes
            // the list we do not pull the data from the controls.

            if (validating && pullData)
            {
                CurrencyManager_PullData();
            }

            // EndCurrentEdit or PullData can cause the list managed by the CurrencyManager to shrink.
            listposition = Math.Min(newPosition, Count - 1);

            if (validating)
            {
                OnCurrentChanged(EventArgs.Empty);
            }

            bool positionChanging = (oldPosition != listposition);
            if (positionChanging && firePositionChange)
            {
                OnPositionChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///  Throws an exception if there is no list.
        /// </summary>
        protected void CheckEmpty()
        {
            if (_dataSource is null || List is null || List.Count == 0)
            {
                throw new InvalidOperationException(SR.ListManagerEmptyList);
            }
        }

        // Will return true if this function changes the position in the list
        private bool CurrencyManager_PushData()
        {
            if (_pullingData)
            {
                return false;
            }

            int initialPosition = listposition;
            if (_lastGoodKnownRow == -1)
            {
                try
                {
                    PushData();
                }
                catch (Exception ex)
                {
                    OnDataError(ex);

                    // Get the first item in the list that is good to push data for now, we assume that there is a row
                    // in the backEnd that is good for all the bindings.
                    FindGoodRow();
                }
                _lastGoodKnownRow = listposition;
            }
            else
            {
                try
                {
                    PushData();
                }
                catch (Exception ex)
                {
                    OnDataError(ex);

                    listposition = _lastGoodKnownRow;
                    PushData();
                }
                _lastGoodKnownRow = listposition;
            }

            return initialPosition != listposition;
        }

        private bool CurrencyManager_PullData()
        {
            bool success = true;
            _pullingData = true;

            try
            {
                PullData(out success);
            }
            finally
            {
                _pullingData = false;
            }

            return success;
        }

        public override void RemoveAt(int index)
        {
            List.RemoveAt(index);
        }

        /// <summary>
        ///  Ends the current edit operation.
        /// </summary>
        public override void EndCurrentEdit()
        {
            if (Count <= 0)
            {
                return;
            }

            if (CurrencyManager_PullData())
            {
                object item = (Position >= 0 && Position < List.Count) ? List[Position] : null;

                if (item is IEditableObject iEditableItem)
                {
                    iEditableItem.EndEdit();
                }

                if (List is ICancelAddNew iListWithCancelAddNewSupport)
                {
                    iListWithCancelAddNewSupport.EndNew(Position);
                }
            }
        }

        private void FindGoodRow()
        {
            int rowCount = List.Count;
            for (int i = 0; i < rowCount; i++)
            {
                listposition = i;
                try
                {
                    PushData();
                }
                catch (Exception ex)
                {
                    OnDataError(ex);
                    continue;
                }
                listposition = i;
                return;
            }

            // If we got here, the list did not contain any rows suitable for the bindings suspend binding and throw.
            SuspendBinding();
            throw new InvalidOperationException(SR.DataBindingPushDataException);
        }

        /// <summary>
        ///  Sets the column to sort by, and the direction of the sort.
        /// </summary>
        internal void SetSort(PropertyDescriptor property, ListSortDirection sortDirection)
        {
            if (List is IBindingList list && list.SupportsSorting)
            {
                list.ApplySort(property, sortDirection);
            }
        }

        /// <summary>
        ///  Gets a <see cref='PropertyDescriptor'/> for a CurrencyManager.
        /// </summary>
        internal PropertyDescriptor GetSortProperty()
            => List is IBindingList list && list.SupportsSorting ? list.SortProperty : null;

        /// <summary>
        ///  Gets the sort direction of a list.
        /// </summary>
        internal ListSortDirection GetSortDirection()
            => (List is IBindingList list) && list.SupportsSorting ? list.SortDirection : ListSortDirection.Ascending;

        /// <summary>
        ///  Find the position of a desired list item.
        /// </summary>
        internal int Find(PropertyDescriptor property, object key, bool keepIndex)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (property != null && (List is IBindingList list) && list.SupportsSearching)
            {
                return list.Find(property, key);
            }

            if (property != null)
            {
                for (int i = 0; i < List.Count; i++)
                {
                    object value = property.GetValue(List[i]);
                    if (key.Equals(value))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        ///  Gets the name of the list.
        /// </summary>
        internal override string GetListName()
            => List is ITypedList list ? list.GetListName(null) : finalType.Name;

        /// <summary>
        ///  Gets the name of the specified list.
        /// </summary>
        protected internal override string GetListName(ArrayList listAccessors)
        {
            if (List is ITypedList list)
            {
                PropertyDescriptor[] properties = new PropertyDescriptor[listAccessors.Count];
                listAccessors.CopyTo(properties, 0);
                return list.GetListName(properties);
            }

            return string.Empty;
        }

        internal override PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
            => ListBindingHelper.GetListItemProperties(List, listAccessors);

        /// <summary>
        ///  Gets the <see cref='PropertyDescriptorCollection'/> for the list.
        /// </summary>
        public override PropertyDescriptorCollection GetItemProperties() => GetItemProperties(null);

        /// <summary>
        ///  Gets the <see cref='PropertyDescriptorCollection'/> for the specified list.
        /// </summary>
        private void List_ListChanged(object sender, ListChangedEventArgs e)
        {
            // If you change the assert below, change the code in the OnCurrentChanged that deals with firing the
            // OnCurrentChanged event.
            Debug.Assert(
                _lastGoodKnownRow == -1 || _lastGoodKnownRow == listposition,
                "if we have a valid lastGoodKnownRow, then it should equal the position in the list");

            ListChangedEventArgs dbe = e.ListChangedType switch
            {
                ListChangedType.ItemMoved when e.OldIndex < 0
                    => new ListChangedEventArgs(ListChangedType.ItemAdded, e.NewIndex, e.OldIndex),
                ListChangedType.ItemMoved when e.NewIndex < 0
                    => new ListChangedEventArgs(ListChangedType.ItemDeleted, e.OldIndex, e.NewIndex),
                _ => e,
            };

            int oldposition = listposition;

            UpdateLastGoodKnownRow(dbe);
            UpdateIsBinding();

            if (List.Count == 0)
            {
                listposition = -1;

                if (oldposition != -1)
                {
                    // If we used to have a current row, but not any more, then report current as changed.
                    OnPositionChanged(EventArgs.Empty);
                    OnCurrentChanged(EventArgs.Empty);
                }

                if (dbe.ListChangedType == ListChangedType.Reset && e.NewIndex == -1)
                {
                    // If the list is reset, then let our users know about it.
                    OnItemChanged(_resetEvent);
                }

                if (dbe.ListChangedType == ListChangedType.ItemDeleted)
                {
                    // If the list is reset, then let our users know about it.
                    OnItemChanged(_resetEvent);
                }

                // We should still fire meta data change notification even when the list is empty.
                if (e.ListChangedType == ListChangedType.PropertyDescriptorAdded ||
                    e.ListChangedType == ListChangedType.PropertyDescriptorDeleted ||
                    e.ListChangedType == ListChangedType.PropertyDescriptorChanged)
                {
                    OnMetaDataChanged(EventArgs.Empty);
                }

                OnListChanged(dbe);
                return;
            }

            _suspendPushDataInCurrentChanged = true;
            try
            {
                switch (dbe.ListChangedType)
                {
                    case ListChangedType.Reset:
                        DataCursorTrace($"System.ComponentModel.ListChangedType.Reset Position: {Position} Count: {List.Count}");

                        if (listposition == -1 && List.Count > 0)
                        {
                            // last false: we don't pull the data from the control when DM changes
                            ChangeRecordState(0, true, false, true, false);
                        }
                        else
                        {
                            ChangeRecordState(Math.Min(listposition, List.Count - 1), true, false, true, false);
                        }

                        UpdateIsBinding(raiseItemChangedEvent: false);
                        OnItemChanged(_resetEvent);
                        break;
                    case ListChangedType.ItemAdded:
                        DataCursorTrace($"System.ComponentModel.ListChangedType.ItemAdded {dbe.NewIndex}");

                        if (dbe.NewIndex <= listposition && listposition < List.Count - 1)
                        {
                            // This means the current row just moved down by one. End the current edit.
                            ChangeRecordState(listposition + 1, true, true, listposition != List.Count - 2, false);
                            UpdateIsBinding();

                            // Refresh the list after we got the item added event.
                            OnItemChanged(_resetEvent);

                            // When we get the itemAdded, and the position was at the end of the list, do the right
                            // thing and notify the positionChanged after refreshing the list.
                            if (listposition == List.Count - 1)
                            {
                                OnPositionChanged(EventArgs.Empty);
                            }

                            break;
                        }
                        else if (dbe.NewIndex == listposition && listposition == List.Count - 1 && listposition != -1)
                        {
                            // The CurrencyManager has a non-empty list.
                            // The position inside the currency manager is at the end of the list and the list still fired an ItemAdded event.
                            // This could be the second ItemAdded event that the DataView fires to signal that the AddNew operation was commited.
                            // We need to fire CurrentItemChanged event so that relatedCurrencyManagers update their lists.
                            OnCurrentItemChanged(EventArgs.Empty);
                        }

                        if (listposition == -1)
                        {
                            // do not call EndEdit on a row that was not there ( position == -1)
                            ChangeRecordState(0, false, false, true, false);
                        }
                        UpdateIsBinding();

                        // Put the call to OnItemChanged after setting the position, so the controls would use the
                        // actual position. If we have a control bound to a dataView, and then we add a row to a the
                        // dataView, then the control will use the old listposition to get the data- which is bad.

                        OnItemChanged(_resetEvent);
                        break;
                    case ListChangedType.ItemDeleted:
                        DataCursorTrace($"System.ComponentModel.ListChangedType.ItemDeleted {dbe.NewIndex}");

                        if (dbe.NewIndex == listposition)
                        {
                            // this means that the current row got deleted.
                            // cannot end an edit on a row that does not exist anymore
                            ChangeRecordState(Math.Min(listposition, Count - 1), true, false, true, false);
                            // put the call to OnItemChanged after setting the position
                            // in the currencyManager, so controls will use the actual position
                            OnItemChanged(_resetEvent);
                            break;
                        }
                        if (dbe.NewIndex < listposition)
                        {
                            // this means the current row just moved up by one.
                            // cannot end an edit on a row that does not exist anymore
                            ChangeRecordState(listposition - 1, true, false, true, false);
                            // put the call to OnItemChanged after setting the position
                            // in the currencyManager, so controls will use the actual position
                            OnItemChanged(_resetEvent);
                            break;
                        }
                        OnItemChanged(_resetEvent);
                        break;
                    case ListChangedType.ItemChanged:
                        DataCursorTrace($"System.ComponentModel.ListChangedType.ItemChanged {dbe.NewIndex}");

                        if (dbe.NewIndex == listposition)
                        {
                            // The current item changed
                            OnCurrentItemChanged(EventArgs.Empty);
                        }

                        OnItemChanged(new ItemChangedEventArgs(dbe.NewIndex));
                        break;
                    case ListChangedType.ItemMoved:
                        DataCursorTrace($"System.ComponentModel.ListChangedType.ItemMoved {dbe.NewIndex}");

                        if (dbe.OldIndex == listposition)
                        {
                            // Current got moved. End the current edit. Make sure there is something that we can end edit.
                            ChangeRecordState(dbe.NewIndex, true, Position > -1 && Position < List.Count, true, false);
                        }
                        else if (dbe.NewIndex == listposition)
                        {
                            // Current was moved. End the current edit. Make sure there is something that we can end edit.
                            ChangeRecordState(dbe.OldIndex, true, Position > -1 && Position < List.Count, true, false);
                        }
                        OnItemChanged(_resetEvent);
                        break;
                    case ListChangedType.PropertyDescriptorAdded:
                    case ListChangedType.PropertyDescriptorDeleted:
                    case ListChangedType.PropertyDescriptorChanged:
                        // reset lastGoodKnownRow because it was computed against property descriptors which changed
                        _lastGoodKnownRow = -1;

                        // In .NET Framework 1.1, metadata changes did not alter current list position. In .NET
                        // Framework 2.0, this behavior is preserved - except it forces the position to stay in valid
                        // range if necessary.

                        if (listposition == -1 && List.Count > 0)
                        {
                            ChangeRecordState(0, true, false, true, false);
                        }
                        else if (listposition > List.Count - 1)
                        {
                            ChangeRecordState(List.Count - 1, true, false, true, false);
                        }

                        // Fire the MetaDataChanged event
                        OnMetaDataChanged(EventArgs.Empty);
                        break;
                }

                // Send the ListChanged notification after the position changed in the list
                OnListChanged(dbe);
            }
            finally
            {
                _suspendPushDataInCurrentChanged = false;
            }

            Debug.Assert(_lastGoodKnownRow == -1 || listposition == _lastGoodKnownRow, "how did they get out of sync?");
        }

        [SRCategory(nameof(SR.CatData))]
        public event EventHandler MetaDataChanged
        {
            add => _onMetaDataChangedHandler += value;
            remove => _onMetaDataChangedHandler -= value;
        }

        /// <summary>
        ///  Causes the CurrentChanged event to occur.
        /// </summary>
        internal protected override void OnCurrentChanged(EventArgs e)
        {
            if (_inChangeRecordState)
            {
                return;
            }

            DataViewTrace($"OnCurrentChanged() {e}");
            int curLastGoodKnownRow = _lastGoodKnownRow;
            bool positionChanged = false;
            if (!_suspendPushDataInCurrentChanged)
            {
                positionChanged = CurrencyManager_PushData();
            }

            if (Count > 0)
            {
                object item = List[Position];
                if (item is IEditableObject editableObject)
                {
                    editableObject.BeginEdit();
                }
            }
            try
            {
                // If CurrencyManager changed position then we have two cases:
                //
                // 1. the previous lastGoodKnownRow was valid: in that case we fell back so do not fire onCurrentChanged
                // 2. The previous lastGoodKnownRow was invalid: we have two cases:
                //      a. FindGoodRow actually found a good row, so it can't be the one before the user changed the
                //         position: fire the onCurrentChanged
                //      b. FindGoodRow did not find a good row: we should have gotten an exception so we should not
                //         even execute this code.
                if (!positionChanged || (positionChanged && curLastGoodKnownRow != -1))
                {
                    onCurrentChangedHandler?.Invoke(this, e);

                    // We fire OnCurrentItemChanged event every time we fire the CurrentChanged + when a property of
                    // the Current item changed.
                    _onCurrentItemChangedHandler?.Invoke(this, e);
                }
            }
            catch (Exception ex)
            {
                OnDataError(ex);
            }
        }

        /// <remarks>
        ///  This method should only be called when the currency manager receives <see cref="ListChangedType.ItemChanged"/>
        ///  and when the index of the <see cref="ListChangedEventArgs"/> equals the position in the currency manager.
        /// </remarks>
        protected internal override void OnCurrentItemChanged(EventArgs e)
            => _onCurrentItemChangedHandler?.Invoke(this, e);

        protected virtual void OnItemChanged(ItemChangedEventArgs e)
        {
            // It is possible that CurrencyManager_PushData will change the position in the list. In that case we have
            // to fire OnPositionChanged event.
            bool positionChanged = false;

            // We should not push the data when we suspend the changeEvents but we should still fire the OnItemChanged
            // event that we get when processing the EndCurrentEdit method.
            if ((e.Index == listposition || (e.Index == -1 && Position < Count)) && !_inChangeRecordState)
            {
                positionChanged = CurrencyManager_PushData();
            }

            DataViewTrace($"OnItemChanged({e.Index}) {e}");

            try
            {
                _onItemChanged?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                OnDataError(ex);
            }

            if (positionChanged)
            {
                OnPositionChanged(EventArgs.Empty);
            }
        }

        private void OnListChanged(ListChangedEventArgs e)
        {
            _onListChanged?.Invoke(this, e);
        }

        // Exists in .NET Framework 1.1
        internal protected void OnMetaDataChanged(EventArgs e)
        {
            _onMetaDataChangedHandler?.Invoke(this, e);
        }

        protected virtual void OnPositionChanged(EventArgs e)
        {
            DataViewTrace($"OnPositionChanged({listposition}) {e}");
            try
            {
                onPositionChangedHandler?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                OnDataError(ex);
            }
        }

        /// <summary>
        ///  Forces a repopulation of the CurrencyManager
        /// </summary>
        public void Refresh()
        {
            if (List.Count > 0)
            {
                if (listposition >= List.Count)
                {
                    _lastGoodKnownRow = -1;
                    listposition = 0;
                }
            }
            else
            {
                listposition = -1;
            }

            List_ListChanged(List, new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        internal void Release()
        {
            UnwireEvents(List);
        }

        /// <summary>
        ///  Resumes binding of component properties to list items.
        /// </summary>
        public override void ResumeBinding()
        {
            _lastGoodKnownRow = -1;
            try
            {
                if (!ShouldBind)
                {
                    ShouldBind = true;

                    // We need to put the listPosition at the beginning of the list if the list is not empty
                    listposition = (List != null && List.Count != 0) ? 0 : -1;
                    UpdateIsBinding();
                }
            }
            catch
            {
                ShouldBind = false;
                UpdateIsBinding();
                throw;
            }
        }

        /// <summary>
        ///  Suspends binding.
        /// </summary>
        public override void SuspendBinding()
        {
            _lastGoodKnownRow = -1;
            if (ShouldBind)
            {
                ShouldBind = false;
                UpdateIsBinding();
            }
        }

        internal void UnwireEvents(IList list)
        {
            if ((list is IBindingList bindingList) && bindingList.SupportsChangeNotification)
            {
                bindingList.ListChanged -= new ListChangedEventHandler(List_ListChanged);
            }
        }

        protected override void UpdateIsBinding()
        {
            UpdateIsBinding(true);
        }

        private void UpdateIsBinding(bool raiseItemChangedEvent)
        {
            bool newBound = List != null && List.Count > 0 && ShouldBind && listposition != -1;
            if (List is null || _bound == newBound)
            {
                return;
            }

            // We will call end edit when moving from bound state to unbounded state
            _bound = newBound;
            int newPos = newBound ? 0 : -1;
            ChangeRecordState(newPos, _bound, (Position != newPos), true, false);
            int numLinks = Bindings.Count;
            for (int i = 0; i < numLinks; i++)
            {
                Bindings[i].UpdateIsBinding();
            }

            if (raiseItemChangedEvent)
            {
                OnItemChanged(_resetEvent);
            }
        }

        private void UpdateLastGoodKnownRow(ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemDeleted:
                    if (e.NewIndex == _lastGoodKnownRow)
                    {
                        _lastGoodKnownRow = -1;
                    }
                    break;
                case ListChangedType.Reset:
                    _lastGoodKnownRow = -1;
                    break;
                case ListChangedType.ItemAdded:
                    if (e.NewIndex <= _lastGoodKnownRow && _lastGoodKnownRow < List.Count - 1)
                    {
                        _lastGoodKnownRow++;
                    }
                    break;
                case ListChangedType.ItemMoved:
                    if (e.OldIndex == _lastGoodKnownRow)
                    {
                        _lastGoodKnownRow = e.NewIndex;
                    }
                    break;
                case ListChangedType.ItemChanged:
                    if (e.NewIndex == _lastGoodKnownRow)
                    {
                        _lastGoodKnownRow = -1;
                    }
                    break;
            }
        }

        internal void WireEvents(IList list)
        {
            if ((list is IBindingList bindingList) && bindingList.SupportsChangeNotification)
            {
                bindingList.ListChanged += new ListChangedEventHandler(List_ListChanged);
            }
        }

        [Conditional("DEBUG")]
        private void DataViewTrace(string message)
            => Debug.WriteLineIf(CompModSwitches.DataView.TraceVerbose, message);

        [Conditional("DEBUG")]
        private void DataCursorTrace(string message)
            => Debug.WriteLineIf(CompModSwitches.DataCursor.TraceVerbose, message);
    }
}
