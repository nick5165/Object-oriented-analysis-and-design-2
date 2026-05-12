package gui;

import database.PostgresTransactionRepository;
import scoring.BadCreditCalculator;
import scoring.CreditCalculator;
import scoring.TransactionRepository;

import javax.swing.*;
import javax.swing.border.EmptyBorder;
import java.awt.*;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

public class BankingApp extends JFrame {

    // ПРИНУДИТЕЛЬНАЯ ЗАГРУЗКА ДРАЙВЕРА
    static {
        try {
            Class.forName("org.postgresql.Driver");
        } catch (ClassNotFoundException e) {
            JOptionPane.showMessageDialog(null, 
                "Драйвер PostgreSQL не найден в памяти!\n\nЗапустите программу строго через терминал с флагом -cp:\njava -cp \".;../postgresql-42.7.2.jar\" gui.BankingApp", 
                "Критическая ошибка", 
                JOptionPane.ERROR_MESSAGE);
            System.exit(1);
        }
    }

    private final JTextArea logArea;
    private final JTextField urlField;
    private final JTextField userField;
    private final JPasswordField passField;
    private final JTextField userIdField;

    // Палитра темной темы
    private final Color BG_DARK = new Color(30, 30, 30);
    private final Color BG_PANEL = new Color(45, 45, 48);
    private final Color TEXT_LIGHT = new Color(220, 220, 220);
    private final Color ACCENT_GREEN = new Color(40, 167, 69);
    private final Color ACCENT_RED = new Color(220, 53, 69);
    private final Font FONT_MAIN = new Font("Segoe UI", Font.PLAIN, 14);

    public BankingApp() {
        setupGlobalTheme();
        
        setTitle("Scoring Control Center | Separated Interface Pattern");
        setSize(1000, 650);
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setLocationRelativeTo(null);
        setLayout(new BorderLayout());
        getContentPane().setBackground(BG_DARK);

        // --- ШАПКА ---
        JLabel headerLabel = new JLabel("  СИСТЕМА СКОРИНГА ПРЕДПРИЯТИЯ (ТОЛЬКО РЕАЛЬНАЯ БД)", SwingConstants.LEFT);
        headerLabel.setFont(new Font("Segoe UI", Font.BOLD, 18));
        headerLabel.setForeground(Color.WHITE);
        headerLabel.setOpaque(true);
        headerLabel.setBackground(new Color(60, 60, 60));
        headerLabel.setBorder(new EmptyBorder(15, 10, 15, 10));
        add(headerLabel, BorderLayout.NORTH);

        // --- ЛЕВАЯ ПАНЕЛЬ (Настройки) ---
        JPanel leftPanel = new JPanel(new GridBagLayout());
        leftPanel.setBackground(BG_PANEL);
        leftPanel.setBorder(new EmptyBorder(20, 20, 20, 20));
        leftPanel.setPreferredSize(new Dimension(350, 0));

        GridBagConstraints gbc = new GridBagConstraints();
        gbc.fill = GridBagConstraints.HORIZONTAL;
        gbc.insets = new Insets(0, 0, 8, 0);
        gbc.weightx = 1.0;
        gbc.gridx = 0;

        urlField = createStyledTextField("jdbc:postgresql://ep-ancient-cloud-am50s93p.c-5.us-east-1.aws.neon.tech/neondb?sslmode=require");
        userField = createStyledTextField("neondb_owner");
        passField = new JPasswordField("");
        styleTextField(passField);
        userIdField = createStyledTextField("user-123");

        addFormRow(leftPanel, "JDBC URL (Neon Cloud):", urlField, gbc);
        addFormRow(leftPanel, "Пользователь БД:", userField, gbc);
        addFormRow(leftPanel, "Пароль БД:", passField, gbc);
        
        gbc.insets = new Insets(20, 0, 8, 0); // Отступ перед клиентом
        addFormRow(leftPanel, "ID Клиента для расчета:", userIdField, gbc);

        // --- ПРАВАЯ ПАНЕЛЬ (Терминал) ---
        logArea = new JTextArea();
        logArea.setEditable(false);
        logArea.setBackground(new Color(20, 20, 20));
        logArea.setForeground(new Color(0, 255, 128)); // Хакерский зеленый
        logArea.setFont(new Font("Consolas", Font.PLAIN, 15));
        logArea.setMargin(new Insets(15, 15, 15, 15));

        JScrollPane scrollPane = new JScrollPane(logArea);
        scrollPane.setBorder(BorderFactory.createLineBorder(new Color(60, 60, 60), 1));
        scrollPane.getVerticalScrollBar().setBackground(BG_PANEL);

        JSplitPane splitPane = new JSplitPane(JSplitPane.HORIZONTAL_SPLIT, leftPanel, scrollPane);
        splitPane.setDividerLocation(350);
        splitPane.setDividerSize(3);
        splitPane.setBorder(null);
        add(splitPane, BorderLayout.CENTER);

        // --- НИЖНЯЯ ПАНЕЛЬ (Кнопки) ---
        JPanel bottomPanel = new JPanel(new FlowLayout(FlowLayout.CENTER, 40, 20));
        bottomPanel.setBackground(BG_DARK);

        JButton btnBad = createStyledButton("ЗАПУСК БЕЗ ПАТТЕРНА", ACCENT_RED);
        JButton btnProd = createStyledButton("ЗАПУСК С ПАТТЕРНОМ", ACCENT_GREEN);

        btnBad.addActionListener(e -> runBadApproach());
        btnProd.addActionListener(e -> runProdApproach());

        bottomPanel.add(btnBad);
        bottomPanel.add(btnProd);

        add(bottomPanel, BorderLayout.SOUTH);
        
        printLog("Драйвер БД успешно загружен.", "INFO");
        printLog("Система инициализирована. Ожидание команд...", "INFO");
    }

    // --- UI HELPER METHODS ---
    private void setupGlobalTheme() {
        UIManager.put("Label.foreground", TEXT_LIGHT);
        UIManager.put("OptionPane.background", BG_PANEL);
        UIManager.put("Panel.background", BG_PANEL);
    }

    private JTextField createStyledTextField(String text) {
        JTextField field = new JTextField(text);
        styleTextField(field);
        return field;
    }

    private void styleTextField(JTextField field) {
        field.setBackground(new Color(60, 60, 60));
        field.setForeground(Color.WHITE);
        field.setCaretColor(Color.WHITE);
        field.setFont(FONT_MAIN);
        field.setBorder(BorderFactory.createCompoundBorder(
                BorderFactory.createLineBorder(new Color(80, 80, 80), 1),
                new EmptyBorder(8, 10, 8, 10)
        ));
    }

    private void addFormRow(JPanel panel, String labelText, JComponent field, GridBagConstraints gbc) {
        JLabel label = new JLabel(labelText);
        label.setFont(new Font("Segoe UI", Font.BOLD, 12));
        label.setForeground(new Color(150, 150, 150));
        panel.add(label, gbc);
        gbc.insets = new Insets(0, 0, 15, 0);
        panel.add(field, gbc);
    }

    private JButton createStyledButton(String text, Color baseColor) {
        JButton button = new JButton(text);
        button.setFont(new Font("Segoe UI", Font.BOLD, 14));
        button.setForeground(Color.WHITE);
        button.setBackground(baseColor);
        button.setFocusPainted(false);
        button.setBorder(new EmptyBorder(15, 40, 15, 40));
        button.setCursor(new Cursor(Cursor.HAND_CURSOR));

        button.addMouseListener(new MouseAdapter() {
            public void mouseEntered(MouseEvent e) {
                button.setBackground(baseColor.brighter());
            }
            public void mouseExited(MouseEvent e) {
                button.setBackground(baseColor);
            }
        });
        return button;
    }

    private void printLog(String message, String level) {
        String time = LocalDateTime.now().format(DateTimeFormatter.ofPattern("HH:mm:ss"));
        String prefix = "";
        if (level.equals("ERROR")) prefix = "[X] ОШИБКА: ";
        if (level.equals("SUCCESS")) prefix = "[V] РЕЗУЛЬТАТ: ";
        if (level.equals("INFO")) prefix = "[i] ИНФО: ";
        
        logArea.append(String.format("%s | %s%s\n", time, prefix, message));
        logArea.setCaretPosition(logArea.getDocument().getLength());
    }

    // --- БИЗНЕС ЛОГИКА ---
    private void runBadApproach() {
        printLog("--------------------------------------------------", "");
        printLog("СТАРТ СЦЕНАРИЯ: БЕЗ ПАТТЕРНА (BadCreditCalculator)", "INFO");
        try {
            String pass = new String(passField.getPassword());
            if (pass.isEmpty()) throw new IllegalArgumentException("Пароль не может быть пустым!");
            
            BadCreditCalculator badCalculator = new BadCreditCalculator(urlField.getText(), userField.getText(), pass);
            String userId = userIdField.getText();
            boolean result = badCalculator.isCreditApproved(userId);
            
            printLog("Клиент: " + userId + " -> " + (result ? "КРЕДИТ ОДОБРЕН" : "ОТКАЗ"), "SUCCESS");
        } catch (Exception ex) {
            printLog(ex.getMessage(), "ERROR");
        }
    }

    private void runProdApproach() {
        printLog("--------------------------------------------------", "");
        printLog("СТАРТ СЦЕНАРИЯ: С ПАТТЕРНОМ (CreditCalculator)", "INFO");
        try {
            String pass = new String(passField.getPassword());
            if (pass.isEmpty()) throw new IllegalArgumentException("Пароль не может быть пустым!");
            
            TransactionRepository sqlRepo = new PostgresTransactionRepository(urlField.getText(), userField.getText(), pass);
            CreditCalculator prodCalculator = new CreditCalculator(sqlRepo);
            
            String userId = userIdField.getText();
            boolean result = prodCalculator.isCreditApproved(userId);
            
            printLog("Клиент: " + userId + " -> " + (result ? "КРЕДИТ ОДОБРЕН" : "ОТКАЗ"), "SUCCESS");
        } catch (Exception ex) {
            printLog(ex.getMessage(), "ERROR");
        }
    }

    public static void main(String[] args) {
        SwingUtilities.invokeLater(() -> new BankingApp().setVisible(true));
    }
}