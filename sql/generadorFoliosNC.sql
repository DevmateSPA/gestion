insert into folios_empresa(empresa_id, tipo, ultimo_folio)
VALUES(1, 'NC', 1182);
insert into folios_empresa(empresa_id, tipo, ultimo_folio)
VALUES(2, 'NC', 0);

DELIMITER $$

CREATE PROCEDURE get_siguiente_folio_nc (
    IN p_empresa_id BIGINT
)
BEGIN
    UPDATE 
		folios_empresa
    SET ultimo_folio = ultimo_folio + 1
    WHERE empresa_id = p_empresa_id AND tipo = 'NC';

    SELECT 
		LPAD(ultimo_folio, 9, '0') AS ultimo_folio
    FROM folios_empresa
    WHERE empresa_id = p_empresa_id AND tipo = 'NC';
END$$

DELIMITER ;