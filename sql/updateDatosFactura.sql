select peli, plan from factura;

UPDATE factura
SET
    peli = CASE
        WHEN peli = 'X' THEN 1
        ELSE 0
    END,
    plan = CASE
        WHEN plan = 'X' THEN 1
        ELSE 0
    END;
    
ALTER TABLE factura
MODIFY peli TINYINT(1) NULL,
MODIFY plan TINYINT(1) NULL;