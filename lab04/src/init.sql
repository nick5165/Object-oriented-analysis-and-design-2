-- 1. Удаляем таблицу, если она была (для чистоты эксперимента)
DROP TABLE IF EXISTS transactions;

-- 2. Создаем таблицу
CREATE TABLE transactions (
    id SERIAL PRIMARY KEY,
    user_id VARCHAR(50) NOT NULL,
    amount DECIMAL(12, 2) NOT NULL,
    type VARCHAR(10) NOT NULL, -- 'INCOME' или 'EXPENSE'
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 3. Заполняем реалистичными данными для 'user-123'
-- Баланс должен получиться: 5000 + 2500 - 1200 - 4500 - 300 = 1500 (Одобрено, так как > 1000)
INSERT INTO transactions (user_id, amount, type, created_at) VALUES 
('user-123', 5000.00, 'INCOME',  '2023-10-01 10:00:00'),
('user-123', 1200.00, 'EXPENSE', '2023-10-05 14:30:00'),
('user-123', 2500.00, 'INCOME',  '2023-10-10 09:15:00'),
('user-123', 4500.00, 'EXPENSE', '2023-10-15 18:00:00'),
('user-123', 300.00,  'EXPENSE', '2023-10-20 12:00:00');

-- Данные для другого пользователя (Баланс 800 - будет отказ)
INSERT INTO transactions (user_id, amount, type, created_at) VALUES 
('user-999', 1000.00, 'INCOME',  '2023-10-01 10:00:00'),
('user-999', 200.00,  'EXPENSE', '2023-10-02 11:00:00');