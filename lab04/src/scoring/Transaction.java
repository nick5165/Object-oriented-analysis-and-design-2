package scoring;

import java.math.BigDecimal;
import java.time.LocalDateTime;

public class Transaction {
    
    public enum Type {
        INCOME, 
        EXPENSE
    }

    private final String id;
    private final BigDecimal amount;
    private final Type type;
    private final LocalDateTime timestamp;

    public Transaction(String id, BigDecimal amount, Type type, LocalDateTime timestamp) {
        this.id = id;
        this.amount = amount;
        this.type = type;
        this.timestamp = timestamp;
    }

    public String getId() {
        return id;
    }

    public BigDecimal getAmount() {
        return amount;
    }

    public Type getType() {
        return type;
    }

    public LocalDateTime getTimestamp() {
        return timestamp;
    }
}