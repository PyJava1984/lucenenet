﻿using System.Globalization;
using System.Text;

namespace Lucene.Net.QueryParsers.Flexible.Messages
{
    /*
     * Licensed to the Apache Software Foundation (ASF) under one or more
     * contributor license agreements.  See the NOTICE file distributed with
     * this work for additional information regarding copyright ownership.
     * The ASF licenses this file to You under the Apache License, Version 2.0
     * (the "License"); you may not use this file except in compliance with
     * the License.  You may obtain a copy of the License at
     *
     *     http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    /// <summary>
    /// Default implementation of Message interface.
    /// For Native Language Support (NLS), system of software internationalization.
    /// </summary>
    public class MessageImpl : IMessage
    {
        private string key;

        private object[] arguments = new object[0];

        public MessageImpl(string key)
        {
            this.key = key;

        }

        public MessageImpl(string key, params object[] args)
            : this(key)
        {
            this.arguments = args;
        }

        public virtual object[] GetArguments()
        {
            return this.arguments;
        }

        public virtual string Key
        {
            get { return this.key; }
        }

        public virtual string GetLocalizedMessage()
        {
            return GetLocalizedMessage(CultureInfo.CurrentUICulture);
        }

        public virtual string GetLocalizedMessage(CultureInfo locale)
        {
            return NLS.GetLocalizedMessage(Key, locale, GetArguments());
        }

        public override string ToString()
        {
            object[] args = GetArguments();
            StringBuilder sb = new StringBuilder(Key);
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    sb.Append(i == 0 ? " " : ", ").Append(args[i]);
                }
            }
            return sb.ToString();
        }
    }
}
