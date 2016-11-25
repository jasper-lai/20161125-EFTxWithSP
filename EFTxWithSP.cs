﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _20161125_EFTxWithSP.Models;

namespace _20161125_EFTxWithSP
{
    public class EFTxWithSP
    {

        // --------------------------------------------------
        // 如何在 SQL Server Profiler 觀察 TRANSACTION 
        // https://weblogs.asp.net/dixin/where-is-transaction-events-in-sql-server-profiler
        // --------------------------------------------------

        /// <summary>
        /// Stored Procedure 在單獨的 Transaction
        /// </summary>
        /// <remarks>
        /// [問題] Stored Procedure 預設會單獨 1 個 Transaction, 即使 EF 沒有 SaveChanges(), 仍會 Commit !
        /// </remarks>
        public void CallSpWithExplicitTx()
        {
            using (EFTestDBEntities ctx = new EFTestDBEntities())
            {
                ObjectParameter orderno = new ObjectParameter("po_order_no", typeof(String));
                ctx.usp_get_order_no(orderno);
                Console.WriteLine(orderno.Value);

                ctx.MyOrders.Add(new MyOrder()
                {
                    OrderNo = orderno.Value.ToString(),
                    ShipName = "jasper",
                    ShipAddress = "taipei",
                    TotalAmt = 1000
                });
                //故意不作 SaveChange(), 查結果, 可以發現 OrderNoGenerators 這個 table 的資料有異動
                //ctx.SaveChanges();
            }
        }

        /// <summary>
        /// Stored Procedure 仍然在單獨的 Transaction
        /// </summary>
        /// <remarks>
        /// [探索1] 試一下 StackOverflow 的解法, 用 SQL Server Profiler 檢查, 沒有 BEGIN TRANSACTION 了, 
        /// 但因為 SQL Server 預設沒有 TRANSACTION 的 INSERT / UPDATE / DELETE 就會作 COMMIT, 所以沒有解決問題
        /// http://stackoverflow.com/questions/19991609/ef6-wraps-every-single-stored-procedure-call-in-its-own-transaction-how-to-prev
        /// </remarks>
        public void CallSpWithImplicitTx()
        {
            using (EFTestDBEntities ctx = new EFTestDBEntities())
            {
                //
                ctx.Configuration.EnsureTransactionsForFunctionsAndCommands = false;
                //
                ObjectParameter orderno = new ObjectParameter("po_order_no", typeof(String));
                ctx.usp_get_order_no(orderno);
                Console.WriteLine(orderno.Value);

                ctx.MyOrders.Add(new MyOrder()
                {
                    OrderNo = orderno.Value.ToString(),
                    ShipName = "jasper",
                    ShipAddress = "taipei",
                    TotalAmt = 1000
                });
                //故意不作 SaveChange(), 查結果, 可以發現 OrderNoGenerators 這個 table 的資料有異動
                //ctx.SaveChanges();
            }
        }


    }
}
