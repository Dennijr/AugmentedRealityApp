using UnityEngine;
using System.Collections;

namespace CustomUI
{
    public static class AppSocial
    {
        //No need of app here
        private const string TWITTER_ADDRESS = "http://twitter.com/intent/tweet";
        private const string TWEET_LANGUAGE = "en"; 

        //Replace with the facebook app id
		private const string FACEBOOK_APP_ID = "1488638048114914";
        private const string FACEBOOK_URL = "http://www.facebook.com/dialog/feed";

        public static void ShareToTwitter()
        {
            string textToDisplay = "Posting from activate AR";
            Application.OpenURL(TWITTER_ADDRESS + "?text=" + WWW.EscapeURL(textToDisplay) + "&amp;lang=" + WWW.EscapeURL(TWEET_LANGUAGE));
        }

        /*
        linkParameter is the link that will be posted to the wall. For example, you could link to your 
        game's website or download page.

        nameParameter is the title of your post. You probably want to briefly describe what it is 
        you're posting (i.e., "I'm playing Game XYZ!"). The name will be a link to the URL described by linkParameter.

        captionParameter is the caption of your post. This appears in small type right below the title 
        of your post. It may be a good idea here to give a bit more detail on what you're posting (for example, "New high score!"). 
        
        descriptionParameter is the body of the message. Here, you can give the bulk of your message, 
        such as the score you attained, an achievement you unlocked, etc. 
        
        pictureParameter is a link to a picture you'd like to include in your post. Keep in mind that 
        pictures must be at least 200px x 200px.
        
        Finally, redirectURIParameter is the page the user will be redirected to after publishing their message.
        This is the parameter that is likeliest to cause you a headache. If you want to go the no-hassle route, just link to http://www.facebook.com/.
        
        Improperly forming your URL or trying to redirect to a domain that is not assigned to your 
        Facebook app are almost always the cause of error 100 and error 191. If you experience either of these, 
        ensure that your URL is being formed correctly by comparing it to the URL redirection example on this page.
        
        If you want to redirect to a page that is outside of Facebook's domain, you'll need to take a 
        couple of extra steps. First, go to your app's page, and select "Settings" from the menu on the left. 
        Click the "Add Platform" button, and select "Website". Under "Site URL", enter the page you want to 
        associate with your app. Now, under "App Domains", enter the domain that page belongs to. So, 
        if you added your Site URL as "www.example.com/index.html", you'll want to enter "www.example.com" 
        under App Domains. Now, your redirect should work as intended.
        */
        //public static void ShareToFacebook(string linkParameter, string nameParameter, string captionParameter, string descriptionParameter, string pictureParameter, string redirectParameter)
        public static void ShareToFacebook()
        {
            string linkParameter = @"http://activatear.com";
            string nameParameter = "Name Parameter ";
            string captionParameter = "Caption";
            string descriptionParameter = "Description Parameter";
			string pictureParameter = @"http://www.activatear.com/activatearvideos/raptor.jpg";
            string redirectParameter = @"http://www.facebook.com/";
            Application.OpenURL(FACEBOOK_URL + "?app_id=" + FACEBOOK_APP_ID +
            "&link=" + WWW.EscapeURL(linkParameter) +
            "&name=" + WWW.EscapeURL(nameParameter) +
            "&caption=" + WWW.EscapeURL(captionParameter) +
            "&description=" + WWW.EscapeURL(descriptionParameter) +
            "&picture=" + WWW.EscapeURL(pictureParameter) +
            "&redirect_uri=" + WWW.EscapeURL(redirectParameter));
        }
    }
}