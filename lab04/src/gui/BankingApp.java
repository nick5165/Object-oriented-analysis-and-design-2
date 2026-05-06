package gui;

import database.PostgresTransactionRepository;
import scoring.BadCreditCalculator;
import scoring.CreditCalculator;
import scoring.Transaction;
import scoring.TransactionRepository;

import javax.swing.*;
import javax.swing.border.TitledBorder;
import java.awt.*;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.Arrays;

public class BankingApp extends JFrame {

    private final JTextArea logArea;
    private final JTextField urlField;
    private final JTextField userField;
    private final JPasswordField passField;
    private final JTextField userIdField;

    public BankingApp() {
        setTitle("Система Скоринга | Демонстрация 'Separated Interface'");
        setSize(850, 600);
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setLocationRelativeTo(null);
        setLayout(new BorderLayout(10, 10));

        JPanel mainPanel = new JPanel(new BorderLayout(10, 10));
        mainPanel.setBorder(BorderFactory.createEmptyBorder(10, 10, 10, 10));

        // --- ПАНЕЛЬ НАСТРОЕК ---
        JPanel settingsPanel = new JPanel(new GridLayout(8, 1, 5, 5));
        settingsPanel.setBorder(BorderFactory.createTitledBorder(
                BorderFactory.createLineBorder(Color.GRAY), "Настройки БД и Пользователя", TitledBorder.LEFT, TitledBorder.TOP));
        settingsPanel.setPreferredSize(new Dimension(300, 0));

        settingsPanel.add(new JLabel("JDBC URL (Облако Neon):"));
        // Твой URL уже вставлен по умолчанию!
        urlField = new JTextField("jdbc:postgresql://ep-ancient-cloud-am50s93p.c-5.us-east-1.aws.neon.tech/neondb?sslmode=require");
        settingsPanel.add(urlField);

        settingsPanel.add(new JLabel("Пользователь БД:"));
        userField = new JTextField("neondb_owner"); // Замени, если у тебя другой логин
        settingsPanel.add(userField);

        settingsPanel.add(new JLabel("Пароль БД:"));
        passField = new JPasswordField(""); // Введешь свой пароль при запуске
        settingsPanel.add(passField);

        settingsPanel.add(new JLabel("ID Клиента для проверки:"));
        userIdField = new JTextField("user-123");
        settingsPanel.add(userIdField);

        // --- ПАНЕЛЬ КНОПОК ---
        JPanel actionPanel = new JPanel(new FlowLayout(FlowLayout.CENTER, 15, 10));
        actionPanel.setBorder(BorderFactory.createTitledBorder("Запуск сценариев (Паттерны)"));

        JButton btnBad = createStyledButton("БЕЗ паттерна (Жесткая связь)", new Color(220, 53, 69));
        JButton btnProd = createStyledButton("С паттерном (Реальная БД)", new Color(40, 167, 69));
        JButton btnMock = createStyledButton("С паттерном (Тест / Без БД)", new Color(0, 123, 255));

        btnBad.addActionListener(e -> runBadApproach());
        btnProd.addActionListener(e -> runProdApproach());
        btnMock.addActionListener(e -> runMockApproach());

        actionPanel.add(btnBad);
        actionPanel.add(btnProd);
        actionPanel.add(btnMock);

        // --- ПАНЕЛЬ ЛОГОВ ---
        logArea = new JTextArea();
        logArea.setEditable(false);
        logArea.setBackground(new Color(43, 43, 43));
        logArea.setForeground(new Color(169, 183, 198));
        logArea.setFont(new Font("Consolas", Font.PLAIN, 14));
        logArea.setMargin(new Insets(10, 10, 10, 10));

        JScrollPane scrollPane = new JScrollPane(logArea);
        scrollPane.setBorder(BorderFactory.createTitledBorder("Консоль вывода / Журнал событий"));

        mainPanel.add(settingsPanel, BorderLayout.WEST);
        mainPanel.add(scrollPane, BorderLayout.CENTER);
        mainPanel.add(actionPanel, BorderLayout.NORTH);

        add(mainPanel);
    }

    private JButton createStyledButton(String text, Color color) {
        JButton button = new JButton(text);
        button.setFocusPainted(false);
        button.setBackground(color);
        button.setForeground(Color.WHITE);
        button.setFont(new Font("Arial", Font.BOLD, 12));
        button.setPreferredSize(new Dimension(220, 35));
        return button;
    }

    private void printLog(String message, boolean isError) {
        if (isError) {
            logArea.append("[ ОШИБКА ] " + message + "\n");
        } else {
            logArea.append(message + "\n");
        }
        logArea.setCaretPosition(logArea.getDocument().getLength());
    }

    private void runBadApproach() {
        printLog("\n=======================================================", false);
        printLog(">>> ЗАПУСК: Архитектура БЕЗ паттерна (BadCreditCalculator)", false);
        try {
            String pass = new String(passField.getPassword());
            BadCreditCalculator badCalculator = new BadCreditCalculator(urlField.getText(), userField.getText(), pass);
            
            String userId = userIdField.getText();
            boolean result = badCalculator.isCreditApproved(userId);
            
            printLog("[ УСПЕХ ] Результат для клиента '" + userId + "': " + (result ? "КРЕДИТ ОДОБРЕН" : "В КРЕДИТЕ ОТКАЗАНО"), false);
        } catch (Exception ex) {
            printLog(ex.getMessage(), true);
        }
    }

    private void runProdApproach() {
        printLog("\n=======================================================", false);
        printLog(">>> ЗАПУСК: Идеальная архитектура (CreditCalculator + PostgreSQL)", false);
        try {
            String pass = new String(passField.getPassword());
            TransactionRepository sqlRepo = new PostgresTransactionRepository(urlField.getText(), userField.getText(), pass);
            CreditCalculator prodCalculator = new CreditCalculator(sqlRepo);
            
            String userId = userIdField.getText();
            boolean result = prodCalculator.isCreditApproved(userId);
            
            printLog("[ УСПЕХ ] Результат из реальной БД для '" + userId + "': " + (result ? "КРЕДИТ ОДОБРЕН" : "В КРЕДИТЕ ОТКАЗАНО"), false);
        } catch (Exception ex) {
            printLog(ex.getMessage(), true);
        }
    }

    private void runMockApproach() {
        printLog("\n=======================================================", false);
        printLog(">>> ЗАПУСК: Идеальная архитектура (CreditCalculator + Mock Test)", false);
        
        TransactionRepository mockRepo = userId -> {
            printLog("    -> [MOCK-СЕРВЕР] Имитация ответа от БД для пользователя: " + userId, false);
            return Arrays.asList(
                new Transaction("m1", new BigDecimal("10000.00"), Transaction.Type.INCOME, LocalDateTime.now()),
                new Transaction("m2", new BigDecimal("2000.00"), Transaction.Type.EXPENSE, LocalDateTime.now())
            );
        };

        CreditCalculator testCalculator = new CreditCalculator(mockRepo);
        String userId = userIdField.getText();
        boolean result = testCalculator.isCreditApproved(userId);
        
        printLog("[ УСПЕХ ] Результат Mock-теста для '" + userId + "': " + (result ? "КРЕДИТ ОДОБРЕН" : "В КРЕДИТЕ ОТКАЗАНО"), false);
    }

    public static void main(String[] args) {
        try { UIManager.setLookAndFeel(UIManager.getSystemLookAndFeelClassName()); } catch (Exception ignored) {}
        SwingUtilities.invokeLater(() -> new BankingApp().setVisible(true));
    }
}