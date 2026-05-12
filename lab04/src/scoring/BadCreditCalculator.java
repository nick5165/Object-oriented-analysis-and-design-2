package scoring;

import database.PostgresTransactionRepository;
import java.math.BigDecimal;
import java.math.RoundingMode;
import java.util.List;

public class BadCreditCalculator {

    private final PostgresTransactionRepository repository;
    private final BigDecimal minApprovalBalance = new BigDecimal("1500.00");

    public BadCreditCalculator(String dbUrl, String dbUser, String dbPass) {
        this.repository = new PostgresTransactionRepository(dbUrl, dbUser, dbPass);
    }

    public boolean isCreditApproved(String userId) {
        List<Transaction> transactions = repository.getTransactionsByUserId(userId);
        
        BigDecimal currentBalance = BigDecimal.ZERO;
        BigDecimal incomeWeight = new BigDecimal("1.10");  
        BigDecimal expenseWeight = new BigDecimal("1.50"); 

        for (Transaction transaction : transactions) {
            if (transaction.getType() == Transaction.Type.INCOME) {
                currentBalance = currentBalance.add(transaction.getAmount().multiply(incomeWeight));
            } else {
                currentBalance = currentBalance.subtract(transaction.getAmount().multiply(expenseWeight));
            }
        }

        return currentBalance.setScale(2, RoundingMode.HALF_UP).compareTo(minApprovalBalance) > 0;
    }
}