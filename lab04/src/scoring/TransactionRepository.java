package scoring;

import java.util.List;

public interface TransactionRepository {
    List<Transaction> getTransactionsByUserId(String userId);
}