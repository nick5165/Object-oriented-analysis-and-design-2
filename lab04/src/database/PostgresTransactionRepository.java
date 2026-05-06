package database;

import scoring.Transaction;
import scoring.TransactionRepository;

import java.math.BigDecimal;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

public class PostgresTransactionRepository implements TransactionRepository {

    private final String url;
    private final String user;
    private final String password;

    public PostgresTransactionRepository(String url, String user, String password) {
        this.url = url;
        this.user = user;
        this.password = password;
        
        // Код регистрации драйвера должен быть СТРОГО внутри фигурных скобок конструктора
        try {
            Class.forName("org.postgresql.Driver");
        } catch (ClassNotFoundException e) {
            System.err.println("Драйвер PostgreSQL не найден! Проверьте наличие JAR файла.");
        }
    }

    @Override
    public List<Transaction> getTransactionsByUserId(String userId) {
        List<Transaction> transactions = new ArrayList<>();
        String sql = "SELECT amount, type, created_at FROM transactions WHERE user_id = ?";

        try (Connection conn = DriverManager.getConnection(url, user, password);
             PreparedStatement stmt = conn.prepareStatement(sql)) {

            stmt.setString(1, userId);
            
            try (ResultSet rs = stmt.executeQuery()) {
                while (rs.next()) {
                    BigDecimal amount = rs.getBigDecimal("amount");
                    Transaction.Type type = Transaction.Type.valueOf(rs.getString("type"));
                    java.time.LocalDateTime timestamp = rs.getTimestamp("created_at").toLocalDateTime();

                    // ID здесь ставим заглушку, так как в интерфейсе он строковый
                    transactions.add(new Transaction("db-id", amount, type, timestamp));
                }
            }
        } catch (SQLException e) {
            throw new RuntimeException("Database error: " + e.getMessage(), e);
        }

        return transactions;
    }
}