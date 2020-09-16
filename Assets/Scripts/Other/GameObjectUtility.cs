using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Utility methods to help deal with game objects.
/// </summary>
public static class GameObjectUtility
{
    /// <summary>
    /// Copies the values from one component to another and returns the updated component.
    /// From https://answers.unity.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    /// <param name="blank">The component of type T to copy values to.</param>
    /// <param name="template">The component of type T to copy values from.</param>
    /// <returns>The updated component.</returns>
    public static T CopyComponentValues<T>(T blank, T template) where T : Component
    {
        Type type = blank.GetType();
        if (type != template.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;

        PropertyInfo[] pinfos = type.GetProperties(flags);

        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(blank, pinfo.GetValue(template, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }

        FieldInfo[] finfos = type.GetFields(flags);

        foreach (var finfo in finfos)
        {
            finfo.SetValue(blank, finfo.GetValue(template));
        }

        return blank as T;
    }
}
