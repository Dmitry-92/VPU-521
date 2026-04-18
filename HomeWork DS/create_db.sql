CREATE SCHEMA test_schema;

SET SEARCH_PATH TO test;

CREATE TABLE table_users(
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    is_driver BOOLEAN DEFAULT FALSE
);

CREATE TABLE table_products(
    id SERIAL PRIMARY KEY,
    item_name TEXT NOT NUll,
    quantity TEXT,
    price NUMERIC DEFAULT 0 CHECK (price >= 0),
    is_purchased BOOLEAN DEFAULT FALSE,
    user_id INTEGER NULL,
    FOREIGN KEY (user_id) REFERENCES table_users(id)
)
CREATE TABLE IF NOT EXISTS table_authors (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    bio TEXT
);