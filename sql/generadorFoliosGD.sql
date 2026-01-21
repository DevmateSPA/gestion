insert into folios_empresa(empresa_id, tipo, ultimo_folio)
VALUES(1, 'GD', 49424);
insert into folios_empresa(empresa_id, tipo, ultimo_folio)
VALUES(2, 'GD', 0);

DELIMITER $$

CREATE PROCEDURE get_siguiente_folio_gd (
    IN p_empresa_id BIGINT
)
BEGIN
    UPDATE 
		folios_empresa
    SET ultimo_folio = ultimo_folio + 1
    WHERE empresa_id = p_empresa_id AND tipo = 'GD';

    SELECT 
		CAST(ultimo_folio AS CHAR) AS ultimo_folio
    FROM folios_empresa
    WHERE empresa_id = p_empresa_id AND tipo = 'GD';
END$$

DELIMITER ;