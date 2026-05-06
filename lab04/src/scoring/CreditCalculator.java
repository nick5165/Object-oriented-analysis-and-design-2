package scoring;

import java.math.BigDecimal;
import java.util.List;

public class CreditCalculator {

    private final TransactionRepository repository;
    private final BigDecimal minApprovalBalance = new BigDecimal("1000.00");

    public CreditCalculator(TransactionRepository repository) {
        this.repository = repository;
    }

    public boolean isCreditApproved(String userId) {
        List<Transaction> transactions = repository.getTransactionsByUserId(userId);
        
        BigDecimal currentBalance = calculateBalance(transactions);
        
        return currentBalance.compareTo(minApprovalBalance) > 0;
    }

    private BigDecimal calculateBalance(List<Transaction> transactions) {
        BigDecimal balance = BigDecimal.ZERO;

        for (Transaction transaction : transactions) {
            if (transaction.getType() == Transaction.Type.INCOME) {
                balance = balance.add(transaction.getAmount());
            } else {
                balance = balance.subtract(transaction.getAmount());
            }
        }

        return balance;
    }
}