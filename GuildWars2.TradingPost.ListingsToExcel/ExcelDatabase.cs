using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace GuildWars2.TradingPost.ListingsToExcel
{
    public class ExcelDatabase
    {
        private const string m_WORKSHEET_NAME = "Listing Info";
        private const string m_WORKSHEET_TABLE_NAME = "Listing Info Table";
        private const string m_WORKSHEET_DATE_FORMAT = "yyyy-mm-dd hh:mm:ss";
        private const string m_WORKSHEET_AMOUNT_FORMAT = "_0_(##_0##_0#0_);[Red]_0(* ##_0##_0#0)";

        public IDictionary<long, ListingInfo> Listings { get; private set; }

        private ExcelDatabase()
        {
            Listings = new Dictionary<long, ListingInfo>();
        }

        public void SaveToFile(string file)
        {
            Stream fileStream;
            ExcelPackage excel = new ExcelPackage();

            if (File.Exists(file))
            {
                fileStream = File.OpenRead(file);
                excel.Load(fileStream);
                fileStream.Close();
                File.Delete(file);
            }

            ExcelWorksheet sheet = excel.Workbook.Worksheets[m_WORKSHEET_NAME];
            if (sheet != null)
                excel.Workbook.Worksheets.Delete(m_WORKSHEET_NAME);

            sheet = excel.Workbook.Worksheets.Add(m_WORKSHEET_NAME);

            int rowIdx = 1;
            sheet.Cells[rowIdx, (int)SheetColumnIdx.LISTING_ID].Value = "Listing ID";
            sheet.Cells[rowIdx, (int)SheetColumnIdx.ITEM_ID].Value = "Item ID";
            sheet.Cells[rowIdx, (int)SheetColumnIdx.ITEM_NAME].Value = "Item Name";
            sheet.Cells[rowIdx, (int)SheetColumnIdx.ITEM_LEVEL].Value = "Item Level";
            sheet.Cells[rowIdx, (int)SheetColumnIdx.ITEM_RARITY].Value = "Item Rarity";
            sheet.Cells[rowIdx, (int)SheetColumnIdx.QUANTITY].Value = "Quantity";
            sheet.Cells[rowIdx, (int)SheetColumnIdx.PRICE].Value = "Price";
            sheet.Cells[rowIdx, (int)SheetColumnIdx.LISTING_TIME].Value = "Listing Time";
            sheet.Cells[rowIdx, (int)SheetColumnIdx.FULFILLED_TIME].Value = "Fulfilled Time";
            sheet.Cells[rowIdx, (int)SheetColumnIdx.TYPE].Value = "Type";
            sheet.Cells[rowIdx, (int)SheetColumnIdx.CANCELLED].Value = "Cancelled";
            sheet.Cells[rowIdx, (int)SheetColumnIdx.FULFILLED].Value = "Fulfilled";
            sheet.Cells[rowIdx, (int)SheetColumnIdx.INVESTMENT_LIQUID].Value = "Investment (Liquid)";
            sheet.Cells[rowIdx, (int)SheetColumnIdx.INVESTMENT_SOLID].Value = "Investment (Solid)";
            sheet.Cells[rowIdx, (int)SheetColumnIdx.INVESTMENT].Value = "Investment";
            sheet.Cells[rowIdx, (int)SheetColumnIdx.REVENUE_UNREALIZED].Value = "Revenue (Unrealized)";
            sheet.Cells[rowIdx, (int)SheetColumnIdx.REVENUE_REALIZED].Value = "Revenue (Realized)";

            // get a sorted list of the elements in the dictionary
            List<ListingInfo> listOfListings = Listings.Values.ToList();
            listOfListings.Sort((a, b) =>
                {
                    return -a.ListingTime.CompareTo(b.ListingTime);
                });

            foreach (ListingInfo listing in listOfListings)
            {
                rowIdx++;

                // data columns
                sheet.Cells[rowIdx, (int)SheetColumnIdx.LISTING_ID].Value = listing.ListingId;
                sheet.Cells[rowIdx, (int)SheetColumnIdx.ITEM_ID].Value = listing.ItemId;
                sheet.Cells[rowIdx, (int)SheetColumnIdx.ITEM_NAME].Value = listing.ItemName;
                sheet.Cells[rowIdx, (int)SheetColumnIdx.ITEM_LEVEL].Value = listing.ItemLevel;
                sheet.Cells[rowIdx, (int)SheetColumnIdx.ITEM_RARITY].Value = listing.ItemRarity;
                sheet.Cells[rowIdx, (int)SheetColumnIdx.QUANTITY].Value = listing.Quantity;
                sheet.Cells[rowIdx, (int)SheetColumnIdx.PRICE].Value = listing.Price;
                sheet.Cells[rowIdx, (int)SheetColumnIdx.LISTING_TIME].Value = listing.ListingTime.ToOADate();
                sheet.Cells[rowIdx, (int)SheetColumnIdx.FULFILLED_TIME].Value = (listing.FulfilledTime == null ? null : (object)listing.FulfilledTime.Value.ToOADate());
                sheet.Cells[rowIdx, (int)SheetColumnIdx.TYPE].Value = listing.Type.ToString();
                sheet.Cells[rowIdx, (int)SheetColumnIdx.CANCELLED].Value = listing.Cancelled;

                // formula columns
                sheet.Cells[rowIdx, (int)SheetColumnIdx.FULFILLED].Formula = string.Format("IF({0} <> \"\", {1}, {2})",
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.FULFILLED_TIME].Address, true.ToString(), false.ToString());
                sheet.Cells[rowIdx, (int)SheetColumnIdx.INVESTMENT_LIQUID].Formula = string.Format("IF(AND({0} = \"{1}\", {2} = {3}, {4} = {5}), {6} * {7}, 0)",
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.TYPE].Address, ListingInfo.ListingType.BUY.ToString(),
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.FULFILLED].Address, false.ToString().ToUpper(),
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.CANCELLED].Address, false.ToString().ToUpper(),
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.QUANTITY].Address, sheet.Cells[rowIdx, (int)SheetColumnIdx.PRICE].Address);
                sheet.Cells[rowIdx, (int)SheetColumnIdx.INVESTMENT_SOLID].Formula = string.Format("IF({0} = \"{1}\", CEILING({2} * {3} * {4}, 1), IF(AND({5} = \"{6}\", {7} = {8}), {9} * {10}, 0))",
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.TYPE].Address, ListingInfo.ListingType.SELL.ToString(),
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.QUANTITY].Address, sheet.Cells[rowIdx, (int)SheetColumnIdx.PRICE].Address, ListingInfo.LISTING_FEE_PERCENT,
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.TYPE].Address, ListingInfo.ListingType.BUY.ToString(),
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.FULFILLED].Address, true.ToString().ToUpper(),
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.QUANTITY].Address, sheet.Cells[rowIdx, (int)SheetColumnIdx.PRICE].Address);
                sheet.Cells[rowIdx, (int)SheetColumnIdx.INVESTMENT].Formula = string.Format("{0} + {1}",
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.INVESTMENT_LIQUID].Address, sheet.Cells[rowIdx, (int)SheetColumnIdx.INVESTMENT_SOLID].Address);
                sheet.Cells[rowIdx, (int)SheetColumnIdx.REVENUE_UNREALIZED].Formula = string.Format("IF(AND({0} = \"{1}\", {2} = {3}, {4} = {5}), FLOOR({6} * {7} * (1.0 - {8}), 1), 0)",
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.TYPE].Address, ListingInfo.ListingType.SELL.ToString(),
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.FULFILLED].Address, false.ToString().ToUpper(),
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.CANCELLED].Address, false.ToString().ToUpper(),
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.QUANTITY].Address, sheet.Cells[rowIdx, (int)SheetColumnIdx.PRICE].Address, ListingInfo.SALE_FEE_PERCENT);
                sheet.Cells[rowIdx, (int)SheetColumnIdx.REVENUE_REALIZED].Formula = string.Format("IF(AND({0} = \"{1}\", {2} = {3}), FLOOR({4} * {5} * (1.0 - {6}), 1), 0)",
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.TYPE].Address, ListingInfo.ListingType.SELL.ToString(),
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.FULFILLED].Address, true.ToString().ToUpper(),
                        sheet.Cells[rowIdx, (int)SheetColumnIdx.QUANTITY].Address, sheet.Cells[rowIdx, (int)SheetColumnIdx.PRICE].Address, ListingInfo.SALE_FEE_PERCENT);
            }

            // format dates properly
            sheet.Column((int)SheetColumnIdx.LISTING_TIME).Style.Numberformat.Format = m_WORKSHEET_DATE_FORMAT;
            sheet.Column((int)SheetColumnIdx.FULFILLED_TIME).Style.Numberformat.Format = m_WORKSHEET_DATE_FORMAT;

            // format amounts properly
            sheet.Column((int)SheetColumnIdx.PRICE).Style.Numberformat.Format = m_WORKSHEET_AMOUNT_FORMAT;
            sheet.Column((int)SheetColumnIdx.INVESTMENT_LIQUID).Style.Numberformat.Format = m_WORKSHEET_AMOUNT_FORMAT;
            sheet.Column((int)SheetColumnIdx.INVESTMENT_SOLID).Style.Numberformat.Format = m_WORKSHEET_AMOUNT_FORMAT;
            sheet.Column((int)SheetColumnIdx.INVESTMENT).Style.Numberformat.Format = m_WORKSHEET_AMOUNT_FORMAT;
            sheet.Column((int)SheetColumnIdx.REVENUE_UNREALIZED).Style.Numberformat.Format = m_WORKSHEET_AMOUNT_FORMAT;
            sheet.Column((int)SheetColumnIdx.REVENUE_REALIZED).Style.Numberformat.Format = m_WORKSHEET_AMOUNT_FORMAT;
            
            // make a pretty table
            ExcelRange tableRange = sheet.Cells[1, 1, sheet.Dimension.End.Row, sheet.Dimension.End.Column];
            ExcelTable table = sheet.Tables.Add(tableRange, m_WORKSHEET_TABLE_NAME);
            table.ShowHeader = true;
            table.TableStyle = TableStyles.Medium20;
            table.ShowTotal = true;
            table.Columns[(int)SheetColumnIdx.INVESTMENT_LIQUID - 1].TotalsRowFunction = RowFunctions.Sum;
            table.Columns[(int)SheetColumnIdx.INVESTMENT_SOLID - 1].TotalsRowFunction = RowFunctions.Sum;
            table.Columns[(int)SheetColumnIdx.INVESTMENT - 1].TotalsRowFunction = RowFunctions.Sum;
            table.Columns[(int)SheetColumnIdx.REVENUE_UNREALIZED - 1].TotalsRowFunction = RowFunctions.Sum;
            table.Columns[(int)SheetColumnIdx.REVENUE_REALIZED - 1].TotalsRowFunction = RowFunctions.Sum;

            // set all the column widths
            for (int colIdx = 1; colIdx <= sheet.Dimension.End.Column; colIdx++)
            {
                // clear formatting on the header cells
                sheet.Cells[1, colIdx].Style.Numberformat.Format = string.Empty;

                // auto-fit that column
                sheet.Column(colIdx).AutoFit();
            }

            // save the new file
            fileStream = File.OpenWrite(file);
            excel.SaveAs(fileStream);
            fileStream.Close();
        }

        public static ExcelDatabase LoadFromFile(string file)
        {
            ExcelDatabase db = new ExcelDatabase();

            if (File.Exists(file))
            {
                Stream fileStream = File.OpenRead(file);
                ExcelPackage excel = new ExcelPackage(fileStream);
                fileStream.Close();

                ExcelWorksheet sheet = excel.Workbook.Worksheets[m_WORKSHEET_NAME];
                if (sheet != null)
                {
                    for (int rowIdx = 2; rowIdx <= sheet.Dimension.End.Row; rowIdx++)
                    {
                        object oListingId = sheet.Cells[rowIdx, (int)SheetColumnIdx.LISTING_ID].Value; // long
                        object oItemId = sheet.Cells[rowIdx, (int)SheetColumnIdx.ITEM_ID].Value; // long
                        object oItemName = sheet.Cells[rowIdx, (int)SheetColumnIdx.ITEM_NAME].Value; // string
                        object oItemLevel = sheet.Cells[rowIdx, (int)SheetColumnIdx.ITEM_LEVEL].Value; // int
                        object oItemRarity = sheet.Cells[rowIdx, (int)SheetColumnIdx.ITEM_RARITY].Value; // string
                        object oQuantity = sheet.Cells[rowIdx, (int)SheetColumnIdx.QUANTITY].Value; // long
                        object oPrice = sheet.Cells[rowIdx, (int)SheetColumnIdx.PRICE].Value; // long
                        object oListingTime = sheet.Cells[rowIdx, (int)SheetColumnIdx.LISTING_TIME].Value; // datetime
                        object oFulfilledTime = sheet.Cells[rowIdx, (int)SheetColumnIdx.FULFILLED_TIME].Value; // datetime
                        object oType = sheet.Cells[rowIdx, (int)SheetColumnIdx.TYPE].Value; // enum
                        object oCancelled = sheet.Cells[rowIdx, (int)SheetColumnIdx.CANCELLED].Value; // bool

                        if (oListingId == null || oItemId == null || oItemName == null || oItemLevel == null || 
                            oItemRarity == null || oQuantity == null || oPrice == null || oListingTime == null ||
                            oType == null)
                            continue;

                        long listingId, itemId, quantity, price;
                        int itemLevel;
                        double listingTimeOA;
                        ListingInfo.ListingType type;

                        if (!long.TryParse(oListingId.ToString(), out listingId) ||
                            !long.TryParse(oItemId.ToString(), out itemId) ||
                            !long.TryParse(oQuantity.ToString(), out quantity) ||
                            !long.TryParse(oPrice.ToString(), out price) ||
                            !int.TryParse(oItemLevel.ToString(), out itemLevel) ||
                            !double.TryParse(oListingTime.ToString(), out listingTimeOA) ||
                            !Enum.TryParse<ListingInfo.ListingType>(oType.ToString(), out type))
                            continue;

                        DateTime listingTime = DateTime.FromOADate(listingTimeOA);

                        string itemName = oItemName.ToString();
                        string itemRarity = oItemRarity.ToString();

                        // BUGFIX: rarity was being set to level for awhile, this attempts to restore the proper
                        //         rarity that was lost
                        BugFixes.Fix_001(ref itemRarity);
                        
                        double fulfilledTimeOA;
                        DateTime? fulfilledTime = null;
                        if (oFulfilledTime != null &&
                            double.TryParse(oFulfilledTime.ToString(), out fulfilledTimeOA))
                            fulfilledTime = DateTime.FromOADate(fulfilledTimeOA);

                        bool cancelled = false;
                        if (oCancelled != null &&
                            !bool.TryParse(oCancelled.ToString(), out cancelled))
                            cancelled = false;

                        db.Listings[listingId] = new ListingInfo()
                            {
                                ListingId = listingId,
                                ItemId = itemId,
                                ItemName = itemName,
                                ItemLevel = itemLevel,
                                ItemRarity = itemRarity,
                                Quantity = quantity,
                                Price = price,
                                ListingTime = listingTime,
                                FulfilledTime = fulfilledTime,
                                Type = type,
                                Cancelled = cancelled
                            };
                    }
                }
            }

            return db;
        }

        private enum SheetColumnIdx
        {
            LISTING_ID = 1,
            ITEM_ID = 2,
            ITEM_NAME = 3,
            ITEM_LEVEL = 4,
            ITEM_RARITY = 5,
            QUANTITY = 6,
            PRICE = 7,
            LISTING_TIME = 8,
            FULFILLED_TIME = 9,
            TYPE = 10,
            CANCELLED = 11,
            FULFILLED = 12,
            INVESTMENT_LIQUID = 13,
            INVESTMENT_SOLID = 14,
            INVESTMENT = 15,
            REVENUE_UNREALIZED = 16,
            REVENUE_REALIZED = 17
        }
    }
}
