using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using StockAPI.DTOs;
using StockAPI.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockAPI.DTOs
{
    public class PurchaseDto
    {


        public DateTime PurchaseDate { get; set; }


        public int ProductId { get; set; }

       
        public int SupplierId { get; set; }

       

        [Precision(18, 2)]
        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }

    }


}
