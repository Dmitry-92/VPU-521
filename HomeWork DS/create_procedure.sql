set search_path to 'bbq_test';

CREATE OR REPLACE PROCEDURE update_product_price(
    p_id INT,
    p_new_price DECIMAL,
    OUT p_old_price DECIMAL
)
    LANGUAGE plpgsql
AS $$
BEGIN
    -- Запоминаем старую цену для выходного параметра
    SELECT price INTO p_old_price FROM table_products WHERE id = p_id;

    -- Обновляем цену
    UPDATE table_products SET price = p_new_price WHERE id = p_id;
END;
$$;

DROP PROCEDURE update_product_price;