package scoring;

import java.math.BigDecimal;
import java.math.RoundingMode;
import java.util.List;

public class CreditCalculator {

    private final TransactionRepository repository;
    private final BigDecimal minApprovalBalance = new BigDecimal("1500.00"); // Повысили порог

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
        
        // Вводим рисковые коэффициенты для умного скоринга
        BigDecimal incomeWeight = new BigDecimal("1.10");  // Доходы дают +10% к весу
        BigDecimal expenseWeight = new BigDecimal("1.50"); // Расходы бьют по скорингу на 50% сильнее

        for (Transaction transaction : transactions) {
            if (transaction.getType() == Transaction.Type.INCOME) {
                balance = balance.add(transaction.getAmount().multiply(incomeWeight));
            } else {
                balance = balance.subtract(transaction.getAmount().multiply(expenseWeight));
            }
        }

        return balance.setScale(2, RoundingMode.HALF_UP);
    }
}