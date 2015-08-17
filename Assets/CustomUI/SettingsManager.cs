using UnityEngine;
using System.Collections;
using System;

namespace CustomUI
{
    public static class SettingsManager
    {
        //All keys for settings
        //Points to note: Keep in mind a setting should have default value
        //as that of the type
        //bool - false
        //int - 0
        //float = 0.0F
        //string - null


        /// <summary>
        /// Get setting value by passing the setting name
        /// Default value - default value of the type of setting
        /// </summary>
        /// <typeparam name="T">Setting type</typeparam>
        /// <param name="key">Setting Name</param>
        /// <returns></returns>
        public static T GetSettings<T>(string key) where T : IConvertible
        {
            if (!PlayerPrefs.HasKey(key))
            {
                return default(T);
            }
            if (typeof(T) == typeof(bool))
            {
                bool value = PlayerPrefs.GetInt(key) == 1 ? true : false;
                return (T)Convert.ChangeType(value, typeof(T));
            }
            else if (typeof(T) == typeof(int))
            {
                int value = PlayerPrefs.GetInt(key);
                return (T)Convert.ChangeType(value, typeof(T));
            }
            else if (typeof(T) == typeof(float))
            {
                float value = PlayerPrefs.GetFloat(key);
                return (T)Convert.ChangeType(value, typeof(T));
            }
            else if (typeof(T) == typeof(string))
            {
                string value = PlayerPrefs.GetString(key);
                return (T)Convert.ChangeType(value, typeof(T));
            }
            return default(T);
        }

        /// <summary>
        /// Set the value for setting
        /// </summary>
        /// <typeparam name="T">Setting type</typeparam>
        /// <param name="key">Name of setting</param>
        /// <param name="value">Value of setting</param>
        public static void SaveSettings<T>(string key, object value)
        {
            if (typeof(T) == typeof(bool))
            {
                int val = (bool)value ? 1 : 0;
                PlayerPrefs.SetInt(key, val);
            }
            else if (typeof(T) == typeof(int))
            {
                PlayerPrefs.SetInt(key, (int)value);
            }
            else if (typeof(T) == typeof(float))
            {
                PlayerPrefs.SetFloat(key, (float)value);
            }
            else if (typeof(T) == typeof(string))
            {
                PlayerPrefs.SetString(key, (string)value);
            }
            PlayerPrefs.Save();
        }
    }
}