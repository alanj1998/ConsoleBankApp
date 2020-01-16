using System;
using SSD.Models;

namespace SSD.Pages
{
    internal sealed class TableHelpers
    {
        internal static void RenderTable(Transactions[] transactions)
        {
            AddLine();
            AddRow("", "Date", "Sender", "Receiver", "Currency", "Amount");
            AddLine();
            for (int i = 0; i < transactions.Length; i++)
            {
                Transactions t = transactions[i];

                AddRow($"{i + 1}.", new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddMilliseconds(t.Timestamp).ToString("dd/MM/yyyy hh:mm tt"), $"{((t.SenderAccount != null) ? t.SenderAccount.FirstName : "")} {((t.SenderAccount != null) ? t.SenderAccount.LastName : "")}", $"{((t.ReceiverAccount != null) ? t.ReceiverAccount.FirstName : "")} {((t.ReceiverAccount != null) ? t.ReceiverAccount.LastName : "")}", $"{t.SmallCurrencyString}", $"{t.Amount}");
                AddLine();
            }
        }

        internal static void AddLine()
        {
            Console.WriteLine("".PadLeft(98, '-'));
        }

        internal static void AddRow(string val1, string val2, string val3, string val4, string val5, string val6)
        {
            Console.WriteLine("|{0,-3}|{1,-20}|{2,-25}|{3,-25}|{4,-8}|{5,-10}|", val1, val2, val3, val4, val5, val6);
        }
    }
}