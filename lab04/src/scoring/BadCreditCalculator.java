package scoring;

import database.PostgresTransactionRepository;
import java.math.BigDecimal;
import java.util.List;

public class BadCreditCalculator {

    // ЖЕСТКАЯ ЗАВИСИМОСТЬ: Класс напрямую знает о PostgreSQL
    private final PostgresTransactionRepository repository;
    private final BigDecimal minApprovalBalance = new BigDecimal("1000.00");

    // Мы передаем настройки, чтобы можно было подключиться к твоей БД
    // Но сам объект БД все равно создается внутри! В этом и суть анти-паттерна.
    public BadCreditCalculator(String dbUrl, String dbUser, String dbPass) {
        this.repository = new PostgresTransactionRepository(dbUrl, dbUser, dbPass);
    }

    public boolean isCreditApproved(String userId) {
        List<Transaction> transactions = repository.getTransactionsByUserId(userId);
        
        BigDecimal currentBalance = BigDecimal.ZERO;

        for (Transaction transaction : transactions) {
            if (transaction.getType() == Transaction.Type.INCOME) {
                currentBalance = currentBalance.add(transaction.getAmount());
            } else {
                currentBalance = currentBalance.subtract(transaction.getAmount());
            }
        }

        return currentBalance.compareTo(minApprovalBalance) > 0;
    }
}