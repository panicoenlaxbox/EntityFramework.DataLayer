﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    // Stefan Cruysberghs, February 2008, http://www.scip.be
    // http://www.scip.be/index.php?Page=ArticlesNET16#Dump
    public static class ObjectStateManagerExtensions
    {
        /// <summary>
        /// Extension method for ObjectStateManager class
        /// Dump all tracking info about the entries in the ObjectStateManager to a string 
        /// </summary>
        /// <param name="manager">ObjectStateManager</param>
        /// <returns>String with tracking info about entries</returns>
        public static string Dump(this ObjectStateManager manager)
        {
            return Dump(manager, null, null, false);
        }

        /// <summary>
        /// Extension method for ObjectStateManager class
        /// Dump all tracking info about the given ObjectStateEntries to a string 
        /// </summary>
        /// <param name="manager">ObjectStateManager</param>
        /// <param name="objectStateEntries">Collection of ObjectStateEntries. If null, then all entities will be displayed</param>
        /// <returns>String with tracking info about entries</returns>
        public static string Dump(this ObjectStateManager manager, IEnumerable<ObjectStateEntry> objectStateEntries)
        {
            return Dump(manager, objectStateEntries, null, false);
        }

        /// <summary>
        /// Extension method for ObjectStateManager class
        /// Dump all tracking info about the given Entity in the ObjectStateManager to a string 
        /// </summary>
        /// <param name="manager">ObjectStateManager</param>
        /// <param name="entityKey">Entity key of given entity. If null, then all entities will be displayed</param>
        /// <returns>String with tracking info about entry</returns>
        public static string Dump(this ObjectStateManager manager, EntityKey entityKey)
        {
            return Dump(manager, null, entityKey, false);
        }

        /// <summary>
        /// Extension method for ObjectStateManager class
        /// Dump all tracking info about the entries in the ObjectStateManager to a HTML string 
        /// </summary>
        /// <param name="manager">ObjectStateManager</param>
        /// <returns>HTML string with tracking info about entries</returns>
        public static string DumpAsHtml(this ObjectStateManager manager)
        {
            return Dump(manager, null, null, true);
        }

        public static string DumpAsHtml(this DbContext context)
        {
            return Dump(GetObjectStateManager(context), null, null, true);
        }

        /// <summary>
        /// Extension method for ObjectStateManager class
        /// Dump all info about the given ObjectStateEntries to a HTML string 
        /// </summary>
        /// <param name="manager">ObjectStateManager</param>
        /// <param name="objectStateEntries">Collection of ObjectStateEntries</param>
        /// <returns>HTML string with tracking info about entries</returns>
        public static string DumpAsHtml(this ObjectStateManager manager,
                                        IEnumerable<ObjectStateEntry> objectStateEntries)
        {
            return Dump(manager, objectStateEntries, null, true);
        }

        /// <summary>
        /// Extension method for ObjectStateManager class
        /// Dump all tracking info about the given Entity in the ObjectStateManager to a HTML string 
        /// </summary>
        /// <param name="manager">ObjectStateManager</param>
        /// <param name="entityKey">Entity key of given entity</param>
        /// <returns>HTML string with tracking info about entry</returns>
        public static string DumpAsHtml(this ObjectStateManager manager, EntityKey entityKey)
        {
            return Dump(manager, null, entityKey, true);
        }

        private static ObjectStateManager GetObjectStateManager(DbContext context)
        {
            return ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager;
        }

        /// <summary>
        /// Private extension method for ObjectStateManager class
        /// Dump all tracking info to a string 
        /// </summary>
        /// <param name="manager">ObjectStateManager</param>
        /// <param name="objectStateEntries">Collection of ObjectStateEntries. If null, then all entities will be displayed</param>
        /// <param name="entityKey">EntityKey of given entity. If null, then all entities will be displayed</param>
        /// <param name="asHtml">Output string as HTML</param>
        /// <returns>String with tracking info about entries</returns>
        private static string Dump(
            this ObjectStateManager manager,
            IEnumerable<ObjectStateEntry> objectStateEntries,
            EntityKey entityKey,
            bool asHtml)
        {
            var dump = new StringBuilder();

            if (entityKey != null)
            {
                objectStateEntries = new List<ObjectStateEntry>();
                (objectStateEntries as List<ObjectStateEntry>).Add(manager.GetObjectStateEntry(entityKey));
            }
            else if (objectStateEntries == null)
            {
                objectStateEntries =
                    manager.GetObjectStateEntries(EntityState.Added)
                        .Union(manager.GetObjectStateEntries(EntityState.Modified))
                        .Union(manager.GetObjectStateEntries(EntityState.Deleted))
                        .Union(manager.GetObjectStateEntries(EntityState.Unchanged));
            }

            dump.AppendFormat("ObjectStateManager entries : # {0}\n", objectStateEntries.Count());

            foreach (var entry in objectStateEntries)
            {
                dump.Append(ObjectStateEntryToString(entry));

                if (entry.State == EntityState.Added)
                {
                    for (var i = 0; i < entry.CurrentValues.FieldCount; i++)
                    {
                        dump.AppendFormat("\n\t- {0} = {1}",
                            entry.CurrentValues.GetName(i),
                            ObjectToString(entry.CurrentValues[i]));
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    foreach (var prop in entry.GetModifiedProperties())
                    {
                        dump.AppendFormat("\n\t- {0} : {1} -> {2}",
                            prop,
                            ObjectToString(entry.OriginalValues[prop]),
                            ObjectToString(entry.CurrentValues[prop]));
                    }
                }
            }

            if (asHtml)
            {
                dump.Replace("\n", "<br />");
                dump.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
            }
            else
            {
                dump.Replace("<b>", "");
                dump.Replace("</b>", "");
            }

            return dump.ToString();
        }

        /// <summary>
        /// Convert an ObjectStateEntry object to a string representation
        /// </summary>
        /// <param name="entry">The given ObjectStateEntry</param>
        /// <returns>The string representation</returns>
        private static string ObjectStateEntryToString(ObjectStateEntry entry)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("\n- <b>{0} ", entry.State);

            if (entry.EntityKey == null)
            {
                if (entry.EntitySet == null)
                    builder.Append("Entity : null </b>[null]");
                else
                    builder.AppendFormat("EntitySet : {0}</b>", entry.EntitySet.Name);
            }
            else
            {
                builder.AppendFormat("Entity : {0} </b>", entry.EntityKey.EntitySetName);

                if (entry.EntityKey.IsTemporary)
                {
                    builder.Append("[Temporary]");
                }
                else
                {
                    foreach (var key in entry.EntityKey.EntityKeyValues)
                    {
                        builder.AppendFormat("[{0} = {1}]", key.Key, ObjectToString(key.Value));
                    }
                }
            }
            return (builder.ToString());
        }

        /// <summary>
        /// Convert an object to a string representation
        /// </summary>
        /// <param name="obj">The given object</param>
        /// <returns>The string representation</returns>
        private static string ObjectToString(object obj)
        {
            if (obj.GetType().Name == "String")
                return $"\"{obj}\"";
            if (obj.ToString() == "")
                return "null";
            return obj.ToString();
        }
    }
}