using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

class Cosmos
{

    public static String GetProdutoApi(string produto)
    {
        var url = "https://api.cosmos.bluesoft.com.br/products?query=" + produto;
        CosmosWebClient wc = new CosmosWebClient();
        string response = wc.DownloadString(url);
        Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(response);
        if (myDeserializedClass.total_count != null && myDeserializedClass.total_count > 0)
        {
            return (myDeserializedClass.products[0].avg_price ==null) ? myDeserializedClass.products[0].price : myDeserializedClass.products[0].avg_price + "|" + myDeserializedClass.products[0].description + "|" + myDeserializedClass.products[0].thumbnail;
        }
        else
        {
            return null;
        }        
    }

    public class CosmosWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            string token = ConfigurationManager.AppSettings["token"].ToString();
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.Headers["X-Cosmos-Token"] = token;
            base.Encoding = System.Text.Encoding.UTF8;
            return request;
        }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class CommercialUnit
    {
        public string type_packaging { get; set; }
        public int? quantity_packaging { get; set; }
        public object ballast { get; set; }
        public object layer { get; set; }

    }

    public class Gtin
    {
        public long? gtin { get; set; }
        public CommercialUnit commercial_unit { get; set; }

    }

    public class Gpc
    {
        public string code { get; set; }
        public string description { get; set; }

    }

    public class Ncm
    {
        public string code { get; set; }
        public string description { get; set; }
        public string full_description { get; set; }

    }

    public class Cest
    {
        public int? id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public int? parent_id { get; set; }

    }

    public class Product
    {
        public string description { get; set; }
        public long? gtin { get; set; }
        public string thumbnail { get; set; }
        public double? width { get; set; }
        public double? height { get; set; }
        public double? length { get; set; }
        public int? net_weight { get; set; }
        public int? gross_weight { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string price { get; set; }
        public double? avg_price { get; set; }
        public double? max_price { get; set; }
        public double? min_price { get; set; }
        public List<Gtin> gtins { get; set; }
        public string origin { get; set; }
        public string barcode_image { get; set; }
        public Gpc gpc { get; set; }
        public Ncm ncm { get; set; }
        public Cest cest { get; set; }

    }

    public class Root
    {
        public List<Product> products { get; set; }
        public int? current_page { get; set; }
        public int? per_page { get; set; }
        public int? total_pages { get; set; }
        public int? total_count { get; set; }

    }

}