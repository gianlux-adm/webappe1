using Ica.SalesOrders.Models;
using Ica.SalesOrders.Web.Models;
using Ica.SalesOrders.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ica.SalesOrders.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LanguageController : BaseController
    {
        public ActionResult Index()
        {
            List<Language> list = Context.Languages.OrderBy(o => o.LanguageName).ToList();
            ListLanguageViewModel llvm = new ListLanguageViewModel();
            llvm.Languages = list;
            return View(llvm);
        }

        

        [HttpGet]
        public ActionResult TranslationPage(string id,string page)
        {
            Language language = Context.Languages.Find(id);
            List<String> pages = Context.Translations.Select(s => s.Page).Distinct().OrderBy(s => s.ToString()).ToList();
            List<Translation> entries = Context.GetTranslationEntries(id);
            List<Translation> entriesEn = Context.GetTranslationEntries("EN");
            EditLanguageTranslationsViewModel evm = EditLanguageTranslationsViewModel.Create(language, pages, entries,entriesEn, null);
            evm.SelectedPage = page;
            return PartialView("EditPage",evm);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            Language l = string.IsNullOrEmpty(id) ? new Language() {  } : Context.Languages.Find(id);
            return View(EditLanguageViewModel.Create(l, null, String.IsNullOrEmpty(id) ? "creation" : "update"));
        }

        [HttpPost]
        public ActionResult Edit(string mode, FormCollection values)
        {
            OperationResult<object> result = new OperationResult<object>();
            Language l = new Language();
            TryUpdateModel(l, "Language", values);

            if (mode.Equals("creation", StringComparison.InvariantCultureIgnoreCase))
            {
                Language tmp = Context.Languages.Find(l.LanguageID);
                if (tmp != null)
                {
                    ModelState.AddModelError("Language.LanguageID", "Already exist an item with the same language id");
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    Language oldLanguage = Context.Languages.Find(l.LanguageID);
                    if (oldLanguage == null)
                    {
                        Context.Languages.Add(l);
                    }
                    else
                    {
                        oldLanguage.LanguageEnabled = l.LanguageEnabled;
                        oldLanguage.LanguageName = l.LanguageName;
                    }
                    String logMessage = String.Format("Language {0} {1} - Status {2}", l.LanguageName, mode.Equals("creation") ? "created" : "modified", l.LanguageEnabled ? "ENABLED" : "DISABLED");
                    Context.AddLog((short)LogActionEnum.EditLanguage, l.LanguageID, null, GetCurrentUserId(), logMessage);

                    Context.SaveChanges();
                    result.Message = "Language saved successfully";
                }
                catch (Exception ex)
                {
                    result.Result = false;
                    result.Message = "Unable to save Language";
                    ModelState.AddModelError("Exception", ex.Message);
                    Context.AddLog((short)LogActionEnum.Exception, null, null, GetCurrentUserId(), "Language/Edit : " + ex.Message);
                }
            }
            else
            {
                result.Message = "Unable to save Language";
            }

            return View(EditLanguageViewModel.Create(l, result, mode));
        }


        [HttpGet]
        public ActionResult EditTranslations(string id)
        {
            Language language = Context.Languages.Find(id);
            List<String> pages = Context.Translations.Select(s=>s.Page).Distinct().OrderBy(s=>s.ToString()).ToList();
            List<Translation> entries = Context.GetTranslationEntries(id);
            List<Translation> entriesEn = Context.GetTranslationEntries("EN");
            return View(EditLanguageTranslationsViewModel.Create(language, pages, entries, entriesEn, null));
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditTranslations(string id, string page, FormCollection values)
        {
            OperationResult<object> result = new OperationResult<object>();
            Language cl = Context.Languages.Include("Translations").Where(w => w.LanguageID.Equals(id)).FirstOrDefault();
            try
            {
                foreach (String key in values.AllKeys)
                {
                    String currentKey = key.TrimStart("li_".ToCharArray());
                    if (key.StartsWith("li_"))
                    {
                        Translation tl = cl.Translations.Where(w => w.Key.Equals(currentKey) && w.Page.Equals(page)).FirstOrDefault();
                        if (tl == null)
                        {
                            Translation l = new Translation();
                            l.Page = page;
                            l.Key = currentKey;
                            l.LanguageID = id;
                            l.Value = values[key];
                            Context.Translations.Add(l);
                        }
                        else
                        {
                            tl.Value = values[key];
                        }
                    }
                }
                Context.SaveChanges();
                String logMessage = String.Format("Language {0} - Page {1} - Translated ", cl.LanguageName, page);
                Context.AddLog((short)LogActionEnum.EditTranslation, id, null, GetCurrentUserId(), logMessage);
                result.Message = "Translation saved successfully";
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Message = "Unable to save translation";
                Context.AddLog((short)LogActionEnum.Exception, null, null, GetCurrentUserId(), "Language/EditTranslations : " + ex.Message);
            }
            List<String> pages = Context.Translations.Select(s=>s.Page).Distinct().OrderBy(s=>s.ToString()).ToList();
            List<Translation> entries = Context.GetTranslationEntries(id);
            List<Translation> entriesEn = Context.GetTranslationEntries("EN");
            EditLanguageTranslationsViewModel evm = EditLanguageTranslationsViewModel.Create(cl, pages, entries,entriesEn, result);
                evm.SelectedPage = page;
            return View(evm);
            
        }
    }
}