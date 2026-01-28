use imprenta;

CREATE TABLE folios_empresa (
    empresa_id BIGINT NOT NULL,
    tipo VARCHAR(20) NOT NULL,
    ultimo_folio INT NOT NULL,
    PRIMARY KEY (empresa_id, tipo)
);

TRUNCATE TABLE folios_empresa;

-- 98323 deberia ser este
INSERT INTO folios_empresa (empresa_id, tipo, ultimo_folio)
SELECT 
    1 AS empresa_id,
    'OT' AS tipo,
    MAX(CAST(folio AS UNSIGNED)) AS ultimo_folio
FROM ordentrabajo
WHERE empresa = 1;

INSERT INTO folios_empresa (empresa_id, tipo, ultimo_folio)
SELECT 
    2 AS empresa_id,
    'OT' AS tipo,
    0;

DELIMITER $$

CREATE PROCEDURE get_siguiente_folio_ot (
    IN p_empresa_id BIGINT
)
BEGIN
    UPDATE 
		folios_empresa
    SET ultimo_folio = ultimo_folio + 1
    WHERE empresa_id = p_empresa_id AND tipo = 'OT';

    SELECT 
		LPAD(ultimo_folio, 9, '0') AS ultimo_folio
    FROM folios_empresa
    WHERE empresa_id = p_empresa_id AND tipo = 'OT';
END$$

DELIMITER ;

-- test CALL get_siguiente_folio(1, 'OT');