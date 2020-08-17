using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CompletOrder.Models
{
    [DataContract]

    public class PrestaKlient
    {
        [DataContract]
        public class Group
        {
            public string id { get; set; }
        }
        [DataContract]

        public class Associations
        {
            public List<Group> groups { get; set; }
        }
        [DataContract]

        public class Customer
        {
            public int id { get; set; }
            public string id_default_group { get; set; }
            public string id_lang { get; set; }
            public string newsletter_date_add { get; set; }
            public object ip_registration_newsletter { get; set; }
            public string last_passwd_gen { get; set; }
            public string secure_key { get; set; }
            public string deleted { get; set; }
            public string passwd { get; set; }
            [DataMember]
            public string lastname { get; set; }
            [DataMember]
            public string firstname { get; set; }
            public string email { get; set; }
            public string id_gender { get; set; }
            public string birthday { get; set; }
            public string newsletter { get; set; }
            public string optin { get; set; }
            public object website { get; set; }
            public object company { get; set; }
            public object siret { get; set; }
            public object ape { get; set; }
            public string outstanding_allow_amount { get; set; }
            public string show_public_prices { get; set; }
            public string id_risk { get; set; }
            public string max_payment_days { get; set; }
            public string active { get; set; }
            public object note { get; set; }
            public string is_guest { get; set; }
            public string id_shop { get; set; }
            public string id_shop_group { get; set; }
            public string date_add { get; set; }
            public string date_upd { get; set; }
            public object reset_password_token { get; set; }
            public string reset_password_validity { get; set; }
            public Associations associations { get; set; }
        }
        [DataContract]

        public class Root
        {
            [DataMember]
            public Customer customer { get; set; }
        }
    }
}
