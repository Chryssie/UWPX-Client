﻿using Logging;
using System;
using System.IO;
using Windows.Storage;

namespace Shared.Classes.SQLite
{
    public abstract class AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "data2.db");
        public static TSSQLiteConnection dB = new TSSQLiteConnection(DB_PATH);

        public const bool RESET_DB_ON_STARTUP = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2017 Created [Fabian Sauter]
        /// </history>
        protected AbstractDBManager()
        {
            if (RESET_DB_ON_STARTUP)
            {
#pragma warning disable CS0162 // Unreachable code detected
                dropTables();
#pragma warning restore CS0162 // Unreachable code detected
            }
            createTables();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Inits the manager.
        /// </summary>
        public virtual void initManager()
        {
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        /// <summary>
        /// Deletes the whole db and recreates an empty one.
        /// Only for testing use resetDB() instead!
        /// </summary>
        protected void deleteDB()
        {
            try
            {
                dB.Close();
                File.Delete(DB_PATH);
            }
            catch (Exception e)
            {
                Logger.Error("Unable to close or delete the DB", e);
            }
            dB = new TSSQLiteConnection(DB_PATH);
        }

        /// <summary>
        /// Drops every table in the db.
        /// </summary>
        protected abstract void dropTables();

        /// <summary>
        /// Creates all required tables.
        /// </summary>
        protected abstract void createTables();

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
