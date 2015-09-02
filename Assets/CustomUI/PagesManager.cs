using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MaterialUI;

namespace CustomUI
{
    public class PagesManager : ScreenManager
    {
        public BasePage[] pages;
        public BasePage homePage;
        [HideInInspector]
        public BasePage currentPage;
        [HideInInspector]
        public Stack<BasePage> lastPages;

        public GameObject toastPrefab;

        private static Toaster toasterObject;

        void Start()
        {
            Init();
            toasterObject = gameObject.GetComponent<Toaster>();
            if (toastPrefab != null) ToastControl.toastPrefab = toastPrefab;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleBackKeyPress();
            }
            else if (Input.GetKeyDown(KeyCode.Menu))
            {
                HandleMenuKeyPress();
            }
        }

        public static void DisplayToast(string message)
        {
            if (toasterObject != null)
            {
                toasterObject.text = message;
                toasterObject.PopupToast();
            }
        }

        public void HandleBackKeyPress()
        {
            if (currentPage != null)
            {
                var e = new CancellationEventArgs()
                {
                    CancelEvent = false
                };
                currentPage.OnBackKeyPress(e);
                if (!e.CancelEvent) Back();
            }
        }

        private void HandleMenuKeyPress()
        {
            if (currentPage != null) currentPage.OnMenuKeyPress();
        }

        private void Init()
        {
            if (pages == null)
                throw new NullReferenceException("Pages not added");
            if (lastPages == null)
                lastPages = new Stack<BasePage>();
            if (homePage == null)
                throw new NullReferenceException("Home page not set");
            if (currentPage == null)
                SetCurrentPage(homePage, NavigationType.New);
        }

        /// <summary>
        /// Set the current page
        /// </summary>
        /// <param name="page"></param>
        private void SetCurrentPage(BasePage page, NavigationType navigationType)
        {
            var e = new NavigationEventArgs()
            {
                navigationType = navigationType
            };
            if (currentPage != null && currentPage.associatedMenu != null)
            {
                currentPage.associatedMenu.UnsetActive();
                currentPage.OnNavigatedFrom(e);
            }
            currentPage = page;
            currentPage.OnNavigatedTo(e);
            if (currentPage == homePage) ClearLastPages();
            if (currentPage.associatedMenu != null)
                currentPage.associatedMenu.SetActive();
        }

        /// <summary>
        /// Get the last page
        /// </summary>
        /// <returns></returns>
        private BasePage GetLastPage()
        {
            if (lastPages.Count > 0)
            {
                return lastPages.Pop();
            }
            return homePage;
        }

        /// <summary>
        /// Set the last page
        /// </summary>
        /// <param name="page"></param>
        private void SetLastPage(BasePage page)
        {
            lastPages.Push(page);
        }

        /// <summary>
        /// Clear the last pages
        /// </summary>
        private void ClearLastPages()
        {
            lastPages.Clear();
        }

        /// <summary>
        /// Set page
        /// </summary>
        /// <param name="newPage"></param>
        private void SetPage(BasePage newPage)
        {
            if (newPage == null) return;
            if (currentPage && currentPage.CurrentState != BasePage.AnimationState.Stationary) return;
            var e = new NavigationEventArgs()
            {
                navigationType = NavigationType.New,
            };
            currentPage.OnNavigatingFrom(e);
            newPage.OnNavigatingTo(e);
            newPage.transform.SetAsLastSibling();
            OnBeforeTransitionBetweenPages();
            newPage.Show(currentPage);
            SetCurrentPage(newPage, NavigationType.New);
        }

        /// <summary>
        /// Get Page from Menu
        /// </summary>
        /// <param name="menuItem"></param>
        /// <returns></returns>
        private BasePage GetPage(MenuItem menuItem)
        {
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i].associatedMenu == menuItem)
                {
                    return pages[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Get page from name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private BasePage GetPage(string name)
        {
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i].screenName == name)
                {
                    return pages[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Navigate to a page without history
        /// Called from navigation drawer(menu manager)
        /// </summary>
        /// <param name="menuItem"></param>
        public void HandleMenuClick(MenuItem menuItem)
        {
            ClearLastPages();
            var newpage = GetPage(menuItem);
            if (newpage != currentPage) SetPage(newpage);
        }

        /// <summary>
        /// Naviage to other page with history recorded
        /// </summary>
        /// <param name="name"></param>
        public void Navigate(string name)
        {
            SetLastPage(currentPage);
            var page = GetPage(name);
            if (page != null) SetPage(page);
        }

        /// <summary>
        /// Navigate back
        /// </summary>
        public new void Back()
        {
            if (currentPage == homePage) return;
            var e = new NavigationEventArgs()
            {
                navigationType = NavigationType.Back
            };
            currentPage.OnNavigatingFrom(e);
            var lastPage = GetLastPage();
            lastPage.OnNavigatingTo(e);
            OnBeforeTransitionBetweenPages();
            lastPage.ShowWithoutTransition();
            currentPage.Hide();
            SetCurrentPage(lastPage, NavigationType.Back);
        }

        public void HideObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }

        private void OnBeforeTransitionBetweenPages()
        {
            ToastControl.HideAllToasts();
        }
    }
}