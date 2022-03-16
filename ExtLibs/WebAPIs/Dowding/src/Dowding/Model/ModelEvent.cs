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
    /// ModelEvent
    /// </summary>
    [DataContract]
    public partial class ModelEvent :  IEquatable<ModelEvent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelEvent" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected ModelEvent() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelEvent" /> class.
        /// </summary>
        /// <param name="Id">UUID (Primary key).</param>
        /// <param name="StartTs">Number representing the epoch timestamp (ms) of the correlatable that first violated a rule (inclusive) (required).</param>
        /// <param name="EndTs">Number representing the epoch timestamp (ms) of the earliest time at which the system had knowledge that a flight stopped violating a rule for the event period (exclusive) (required).</param>
        /// <param name="RuleKey">String representing a keyed name of rule the event applies to (required).</param>
        /// <param name="Message">String representing the event message (required).</param>
        /// <param name="ContactId">UUID of Contact that this event is attached to (required).</param>
        public ModelEvent(string Id = null, decimal? StartTs = null, decimal? EndTs = null, string RuleKey = null, string Message = null, string ContactId = null)
        {
            // to ensure "StartTs" is required (not null)
            if (StartTs == null)
            {
                throw new InvalidDataException("StartTs is a required property for ModelEvent and cannot be null");
            }
            else
            {
                this.StartTs = StartTs;
            }
            // to ensure "EndTs" is required (not null)
            if (EndTs == null)
            {
                throw new InvalidDataException("EndTs is a required property for ModelEvent and cannot be null");
            }
            else
            {
                this.EndTs = EndTs;
            }
            // to ensure "RuleKey" is required (not null)
            if (RuleKey == null)
            {
                throw new InvalidDataException("RuleKey is a required property for ModelEvent and cannot be null");
            }
            else
            {
                this.RuleKey = RuleKey;
            }
            // to ensure "Message" is required (not null)
            if (Message == null)
            {
                throw new InvalidDataException("Message is a required property for ModelEvent and cannot be null");
            }
            else
            {
                this.Message = Message;
            }
            // to ensure "ContactId" is required (not null)
            if (ContactId == null)
            {
                throw new InvalidDataException("ContactId is a required property for ModelEvent and cannot be null");
            }
            else
            {
                this.ContactId = ContactId;
            }
            this.Id = Id;
        }
        
        /// <summary>
        /// UUID (Primary key)
        /// </summary>
        /// <value>UUID (Primary key)</value>
        [DataMember(Name="id", EmitDefaultValue=false)]
        public string Id { get; set; }
        /// <summary>
        /// Number representing the epoch timestamp (ms) of the correlatable that first violated a rule (inclusive)
        /// </summary>
        /// <value>Number representing the epoch timestamp (ms) of the correlatable that first violated a rule (inclusive)</value>
        [DataMember(Name="start_ts", EmitDefaultValue=false)]
        public decimal? StartTs { get; set; }
        /// <summary>
        /// Number representing the epoch timestamp (ms) of the earliest time at which the system had knowledge that a flight stopped violating a rule for the event period (exclusive)
        /// </summary>
        /// <value>Number representing the epoch timestamp (ms) of the earliest time at which the system had knowledge that a flight stopped violating a rule for the event period (exclusive)</value>
        [DataMember(Name="end_ts", EmitDefaultValue=false)]
        public decimal? EndTs { get; set; }
        /// <summary>
        /// String representing a keyed name of rule the event applies to
        /// </summary>
        /// <value>String representing a keyed name of rule the event applies to</value>
        [DataMember(Name="rule_key", EmitDefaultValue=false)]
        public string RuleKey { get; set; }
        /// <summary>
        /// String representing the event message
        /// </summary>
        /// <value>String representing the event message</value>
        [DataMember(Name="message", EmitDefaultValue=false)]
        public string Message { get; set; }
        /// <summary>
        /// UUID of Contact that this event is attached to
        /// </summary>
        /// <value>UUID of Contact that this event is attached to</value>
        [DataMember(Name="contact_id", EmitDefaultValue=false)]
        public string ContactId { get; set; }
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ModelEvent {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  StartTs: ").Append(StartTs).Append("\n");
            sb.Append("  EndTs: ").Append(EndTs).Append("\n");
            sb.Append("  RuleKey: ").Append(RuleKey).Append("\n");
            sb.Append("  Message: ").Append(Message).Append("\n");
            sb.Append("  ContactId: ").Append(ContactId).Append("\n");
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
            return this.Equals(obj as ModelEvent);
        }

        /// <summary>
        /// Returns true if ModelEvent instances are equal
        /// </summary>
        /// <param name="other">Instance of ModelEvent to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ModelEvent other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return 
                (
                    this.Id == other.Id ||
                    this.Id != null &&
                    this.Id.Equals(other.Id)
                ) && 
                (
                    this.StartTs == other.StartTs ||
                    this.StartTs != null &&
                    this.StartTs.Equals(other.StartTs)
                ) && 
                (
                    this.EndTs == other.EndTs ||
                    this.EndTs != null &&
                    this.EndTs.Equals(other.EndTs)
                ) && 
                (
                    this.RuleKey == other.RuleKey ||
                    this.RuleKey != null &&
                    this.RuleKey.Equals(other.RuleKey)
                ) && 
                (
                    this.Message == other.Message ||
                    this.Message != null &&
                    this.Message.Equals(other.Message)
                ) && 
                (
                    this.ContactId == other.ContactId ||
                    this.ContactId != null &&
                    this.ContactId.Equals(other.ContactId)
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
                if (this.Id != null)
                    hash = hash * 59 + this.Id.GetHashCode();
                if (this.StartTs != null)
                    hash = hash * 59 + this.StartTs.GetHashCode();
                if (this.EndTs != null)
                    hash = hash * 59 + this.EndTs.GetHashCode();
                if (this.RuleKey != null)
                    hash = hash * 59 + this.RuleKey.GetHashCode();
                if (this.Message != null)
                    hash = hash * 59 + this.Message.GetHashCode();
                if (this.ContactId != null)
                    hash = hash * 59 + this.ContactId.GetHashCode();
                return hash;
            }
        }
    }

}
