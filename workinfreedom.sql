create database workinfreedom;

use workinfreedom;
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    familiya VARCHAR(50) NOT NULL,
    imya VARCHAR(50) NOT NULL,
    telefon VARCHAR(20),
    login VARCHAR(255) NOT NULL UNIQUE,
    parol VARCHAR(255) NOT NULL,
    role ENUM('Администратор', 'Фрилансер', 'Заказчик') NOT NULL DEFAULT 'Заказчик'
);

use workinfreedom;
CREATE TABLE yazyki (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nazvanie VARCHAR(255) NOT NULL UNIQUE
);
INSERT INTO yazyki (nazvanie) VALUES
  ('Python'),
  ('Java'),
  ('JavaScript'),
  ('C#'),
  ('C++'),
  ('PHP'),
  ('Swift'),
  ('Kotlin'),
  ('Go'),
  ('Ruby'),
  ('Rust'),
  ('TypeScript'),
  ('HTML'),
  ('CSS'),
  ('SQL');

use workinfreedom;
CREATE TABLE kategorii (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nazvanie VARCHAR(255) NOT NULL UNIQUE
);
INSERT INTO kategorii (nazvanie) VALUES 
('Корпоративные сайты'),
('Интернет-магазины'),
('Веб-приложения'),
('Лэндинги и посадочные страницы'),
('Приложения для iOS'),
('Приложения для Android'),
('Кросс-платформенные приложения'),
('Платформы для ведения блогов'),
('Системы управления для новостных порталов'),
('Экосистемы для создания сайтов'),
('Видеоигры (ПК, консоли, мобильные)'),
('Симуляторы'),
('Образовательные игры'),
('Умные устройства'),
('Облачные решения для сбора данных'),
('Приложения для управления IoT-устройствами'),
('ERP-системы'),
('CRM-системы'),
('Управление проектами и задачами'),
('Системы для обработки и анализа больших данных'),
('BI-платформы'),
('Дата-складирование'),
('Системы рекомендаций'),
('Чат-боты'),
('Решения для распознавания изображений'),
('Системы защиты данных'),
('Мониторинг безопасности'),
('Обучение сотрудников по безопасности'),
('Облачный хостинг и вычисления'),
('SaaS-приложения'),
('Платформы для разработки и тестирования'),
('Платформы для онлайн-продаж'),
('Решения для оплаты и обработки заказов'),
('Платформы для онлайн-обучения'),
('Электронные курсы'),
('Решения для управления обучением (LMS)');

use workinfreedom;
CREATE TABLE vakansii (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nazvanie VARCHAR(255) NOT NULL,
  kategoria_id INT,
  opisanie TEXT,
  yazik_id INT,
  srok_vipolnenia DATE,
  byudzhet DECIMAL(10,2),
  user_id INT,
  status TINYINT(1) DEFAULT 0,
  FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
  FOREIGN KEY (kategoria_id) REFERENCES kategorii(id) ON DELETE CASCADE,
  FOREIGN KEY (yazik_id) REFERENCES yazyki(id) ON DELETE CASCADE
);

CREATE TABLE specializations (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nazvanie VARCHAR(255) NOT NULL UNIQUE
);
INSERT INTO specializations (nazvanie) VALUES
('Web-разработка'),
('Мобильная разработка'),
('Data Science'),
('Искусственный интеллект'),
('Безопасность информации'),
('Системный администратор'),
('DevOps-инженер'),
('QA-инженер'),
('Аналитик данных'),
('Блокчейн-разработка'),
('UI/UX-дизайнер'),
('Front-end разработчик'),
('Back-end разработчик'),
('Игравая разработка'),
('Cloud-инженер'),
('Сетевой инженер'),
('Машинное обучение'),
('Инженер по данным'),
('Робототехника'),
('Идентификация и защита данных');

USE workinfreedom;
CREATE TABLE zayavki (
  id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  specialization_id INT NOT NULL,
  vakansiya_id INT NOT NULL,
  opyt_rabot TEXT,
  FOREIGN KEY (user_id) REFERENCES users(id),
  FOREIGN KEY (specialization_id) REFERENCES specializations(id),
  FOREIGN KEY (vakansiya_id) REFERENCES vakansii(id)
);
