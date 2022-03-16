/* 
 * Dowding HTTP REST API
 *
 * The Dowding HTTP REST API allows you to add and retrieve contact data from Dowding as well as perform other peripheral functions.
 *
 * OpenAPI spec version: 1.0.0
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dowding.Model
{
    /// <summary>
    /// ZoneDto
    /// </summary>
    [DataContract]
    public partial class ZoneDto :  IEquatable<ZoneDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneDto" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected ZoneDto() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneDto" /> class.
        /// </summary>
        /// <param name="Geom">GeoJSON geometry object representing the shape of this zone (required).</param>
        /// <param name="Name">String representing the name of this zone (required).</param>
        /// <param name="ZoneSeverityId">UUID of the ZONE_SEVERITY for this ZONE (required).</param>
        public ZoneDto(Object Geom = null, string Name = null, string ZoneSeverityId = null)
        {
            // to ensure "Geom" is required (not null)
            if (Geom == null)
            {
                throw new InvalidDataException("Geom is a required property for ZoneDto and cannot be null");
            }
            else
            {
                this.Geom = Geom;
            }
            // to ensure "Name" is required (not null)
            if (Name == null)
            {
                throw new InvalidDataException("Name is a required property for ZoneDto and cannot be null");
            }
            else
            {
                this.Name = Name;
            }
            // to ensure "ZoneSeverityId" is required (not null)
            if (ZoneSeverityId == null)
            {
                throw new InvalidDataException("ZoneSeverityId is a required property for ZoneDto and cannot be null");
            }
            else
            {
                this.ZoneSeverityId = ZoneSeverityId;
            }
        }
        
        /// <summary>
        /// GeoJSON geometry object representing the shape of this zone
        /// </summary>
        /// <value>GeoJSON geometry object representing the shape of this zone</value>
        [DataMember(Name="geom", EmitDefaultValue=false)]
        public Object Geom { get; set; }
        /// <summary>
        /// String representing the name of this zone
        /// </summary>
        /// <value>String representing the name of this zone</value>
        [DataMember(Name="name", EmitDefaultValue=false)]
        public string Name { get; set; }
        /// <summary>
        /// UUID of the ZONE_SEVERITY for this ZONE
        /// </summary>
        /// <value>UUID of the ZONE_SEVERITY for this ZONE</value>
        [DataMember(Name="zone_severity_id", EmitDefaultValue=false)]
        public string ZoneSeverityId { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ZoneDto {\n");
            sb.Append("  Geom: ").Append(Geom).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  ZoneSeverityId: ").Append(ZoneSeverityId).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            return this.Equals(obj as ZoneDto);
        }

        /// <summary>
        /// Returns true if ZoneDto instances are equal
        /// </summary>
        /// <param name="other">Instance of ZoneDto to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ZoneDto other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return 
                (
                    this.Geom == other.Geom ||
                    this.Geom != null &&
                    this.Geom.Equals(other.Geom)
                ) && 
                (
                    this.Name == other.Name ||
                    this.Name != null &&
                    this.Name.Equals(other.Name)
                ) && 
                (
                    this.ZoneSeverityId == other.ZoneSeverityId ||
                    this.ZoneSeverityId != null &&
                    this.ZoneSeverityId.Equals(other.ZoneSeverityId)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = 41;
                // Suitable nullity checks etc, of course :)
                if (this.Geom != null)
                    hash = hash * 59 + this.Geom.GetHashCode();
                if (this.Name != null)
                    hash = hash * 59 + this.Name.GetHashCode();
                if (this.ZoneSeverityId != null)
                    hash = hash * 59 + this.ZoneSeverityId.GetHashCode();
                return hash;
            }
        }
    }

}
