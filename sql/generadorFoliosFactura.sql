-- 'N00050293' o el que sea el ultimo folio
insert into folios_empresa(empresa_id, tipo, ultimo_folio)
VALUES(1, 'FA', 50293);
insert into folios_empresa(empresa_id, tipo, ultimo_folio)
VALUES(2, 'FA', 0);

DELIMITER $$

CREATE PROCEDURE get_siguiente_folio_fa (
    IN p_empresa_id BIGINT
)
BEGIN
    UPDATE 
		folios_empresa
    SET ultimo_folio = ultimo_folio + 1
    WHERE empresa_id = p_empresa_id AND tipo = 'FA';

    SELECT 
		CONCAT('N', LPAD(ultimo_folio, 8, '0')) AS ultimo_folio
    FROM folios_empresa
    WHERE empresa_id = p_empresa_id AND tipo = 'FA';
END$$

DELIMITER ;