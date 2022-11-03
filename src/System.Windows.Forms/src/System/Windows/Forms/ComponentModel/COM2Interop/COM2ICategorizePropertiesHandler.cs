// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2ICategorizePropertiesHandler : Com2ExtendedBrowsingHandler
    {
        public override Type Interface => typeof(ICategorizeProperties.Interface);

        private static unsafe string? GetCategoryFromObject(object? obj, int dispid)
        {
            using var categorize = ComHelpers.GetComScope<ICategorizeProperties>(obj, out bool success);
            if (!success)
            {
                return null;
            }

            PROPCAT categoryId = 0;
            if (categorize.Value->MapPropertyToCategory(dispid, &categoryId) != HRESULT.S_OK)
            {
                return null;
            }

            return categoryId switch
            {
                PROPCAT.Nil => string.Empty,
                PROPCAT.Misc => SR.PropertyCategoryMisc,
                PROPCAT.Font => SR.PropertyCategoryFont,
                PROPCAT.Position => SR.PropertyCategoryPosition,
                PROPCAT.Appearance => SR.PropertyCategoryAppearance,
                PROPCAT.Behavior => SR.PropertyCategoryBehavior,
                PROPCAT.Data => SR.PropertyCategoryData,
                PROPCAT.List => SR.PropertyCategoryList,
                PROPCAT.Text => SR.PropertyCategoryText,
                PROPCAT.Scale => SR.PropertyCategoryScale,
                PROPCAT.DDE => SR.PropertyCategoryDDE,
                _ => GetCategoryName(categorize, categoryId)
            };

            static string? GetCategoryName(ICategorizeProperties* categorize, PROPCAT categoryId)
            {
                using BSTR categoryName = default;
                return categorize->GetCategoryName(categoryId, (int)PInvoke.GetThreadLocale(), &categoryName).Succeeded
                    ? categoryName.ToString()
                    : null;
            }
        }

        public override void SetupPropertyHandlers(Com2PropertyDescriptor[]? propDesc)
        {
            if (propDesc is null)
            {
                return;
            }

            for (int i = 0; i < propDesc.Length; i++)
            {
                propDesc[i].QueryGetBaseAttributes += new GetAttributesEventHandler(OnGetAttributes);
            }
        }

        private void OnGetAttributes(Com2PropertyDescriptor sender, GetAttributesEvent attrEvent)
        {
            string? category = GetCategoryFromObject(sender.TargetObject, sender.DISPID);

            if (category?.Length > 0)
            {
                attrEvent.Add(new CategoryAttribute(category));
            }
        }
    }
}
