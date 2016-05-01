/*****************************************************************************************
 *  p2p-player
 *  An audio player developed in C# based on a shared base to obtain the music from.
 * 
 *  Copyright (C) 2010-2011 Dario Mazza, Sebastiano Merlino
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as
 *  published by the Free Software Foundation, either version 3 of the
 *  License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  Dario Mazza (dariomzz@gmail.com)
 *  Sebastiano Merlino (etr@pensieroartificiale.com)
 *  Full Source and Documentation available on Google Code Project "p2p-player", 
 *  see <http://code.google.com/p/p2p-player/>
 *
 ******************************************************************************************/

/*****************************************************************************************
 *  p2p-player
 *  An audio player developed in C# based on a shared base to obtain the music from.
 * 
 *  Copyright (C) 2010-2011 Dario Mazza, Sebastiano Merlino
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as
 *  published by the Free Software Foundation, either version 3 of the
 *  License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  Dario Mazza (dariomzz@gmail.com)
 *  Sebastiano Merlino (etr@pensieroartificiale.com)
 *  Full Source and Documentation available on Google Code Project "p2p-player", 
 *  see <http://code.google.com/p/p2p-player/>
 *
 ******************************************************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Persistence.Tag;
using System.Runtime.Serialization;

namespace Persistence
{
    /// <summary>
    /// Class that describes a Resource shared with Kademlia.
    /// A Kademlia Resource contains information about Tag and a list of suppliers (stored as set of Dht Elements).
    /// </summary>
    [Serializable]
    [DataContractAttribute]
    public class KademliaResource: ILoadable
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public KademliaResource()
        {
            this.Urls = new HashSet<DhtElement>();

        }
        /// <summary>
        /// Constructor that initializes the suppliers list with the given Urls and fill tag information with
        /// data read from the filename.
        /// </summary>
        /// <param name="filename">Filename from whom extract the tag information</param>
        /// <param name="urls">List of supplier urls</param>
        public KademliaResource(string filename, params DhtElement[] urls) :this() {
            if (urls.Length != 0)
            {
                foreach(DhtElement e in urls)
                {
                    this.Urls.Add(e);
                }
//                this.Urls.Union<DhtElement>(urls, new DhtElementComparer());
            }
        }
        /// <summary>
        /// Main Constructor of the resource. This initializes the fields with the given values
        /// </summary>
        /// <param name="tag">Tag Information</param>
        /// <param name="urls">List of supplier urls</param>
        public KademliaResource(params DhtElement[] urls) : this()
        {
            if (urls.Length != 0)
            {
                foreach (DhtElement e in urls)
                {
                    this.Urls.Add(e);
                }
//                this.Urls.Union<DhtElement>(urls, new DhtElementComparer());
            }
        }
        /// <summary>
        /// Method used for serialization purpose
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public KademliaResource(SerializationInfo info, StreamingContext ctxt) : this()
        {
            this.Urls = (HashSet<DhtElement>)info.GetValue("Urls", typeof(HashSet<DhtElement>));
        }

        /// <summary>
        /// Set of Dht Elements that are the suppliers for the resource
        /// </summary>
        [DataMemberAttribute]
        public HashSet<DhtElement> Urls
        {
            get;
            set;
        }        
        #region ILoadable

        /// <summary>
        /// Returns the current object because this is ready to be loaded in the repository.
        /// </summary>
        /// <returns>The object as database type</returns>
        public dynamic GetAsDatabaseType()
        {
            return this;
        }
        /// <summary>
        /// Loads data from a dynamic object loaded from repository
        /// </summary>
        /// <param name="data">Object loaded from Repository</param>
        /// <returns>True if the data has been successfully loaded, false otherwise</returns>
        public bool LoadFromDatabaseType(dynamic data)
        {
            this.Urls.Clear();
            this.Urls.UnionWith(data.Urls);
            return true;
        }
        /// <summary>
        /// Returns the type reference for the database type
        /// </summary>
        /// <returns>The current type</returns>
        public Type GetDatabaseType()
        {
            return this.GetType();
        }
        #endregion

        /// <summary>
        /// Method used for serialization purpose
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Urls", this.Urls);
        }

        /// <summary>
        /// Method that merges the Dht Elements of the current resource with the ones of an another resource.
        /// </summary>
        /// <param name="other">The other resource</param>
        /// <returns></returns>
        public bool MergeTo(KademliaResource other)
        {
            Console.WriteLine("Merged!");
            foreach (DhtElement e in other.Urls)
            {
                this.Urls.Add(e);
            }
            return true;
        }
    }
}
