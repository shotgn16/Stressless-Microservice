using Microsoft.AspNetCore.Mvc;
using ServiceStack;
using System.Data.SQLite;
using System.Linq;

namespace Stressless_Service.Database
{
    public class TransactionController : Controller
    {
        private ILogger<TransactionController> _logger;
        public Queue<SQLiteTransaction> _sQLiteTransactions;

        public TransactionController(ILogger<TransactionController> logger, Queue<SQLiteTransaction> sQLiteTransactions)
        {
            _logger = logger;
            _sQLiteTransactions = sQLiteTransactions;
        }

        public async Task AddTransaction(SQLiteTransaction transaction)
        {
            try
            {
                if (transaction != null) {
                    _sQLiteTransactions.Append(transaction);
                }

                else if (transaction == null) {
                    throw new ArgumentException($"Invalid transaction\n{transaction.ToJson()}\n\n{DateTime.Now}");
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        /// <summary>
        /// * Designed to execute sQLiteTransactions
        /// * Will check the '_sQLiteTransactions' every '1 Second' for any new transactions
        /// *  If found - Will execute them one by one, iterating through the list.
        /// </summary>
        public async Task ExecuteTransactions()
        {
            try
            {
                if (_sQLiteTransactions != null) {
                    foreach (var transaction in _sQLiteTransactions) {
                        transaction.Commit();
                    }
                }

                else if (_sQLiteTransactions == null) {
                    _logger.LogInformation("No Transactions found... Will try again in 2 minutes...");
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

    }
}
