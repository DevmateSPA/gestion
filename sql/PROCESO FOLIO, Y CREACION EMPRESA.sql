-- Tabla base para los folios
CREATE TABLE folios_empresa (
    empresa_id BIGINT NOT NULL,
    tipo VARCHAR(20) NOT NULL,
    ultimo_folio INT NOT NULL,
    PRIMARY KEY (empresa_id, tipo)
);

-- Datos para Orden de Trabajo
-- 98323 deberia ser este o el ultimo
INSERT INTO folios_empresa (empresa_id, tipo, ultimo_folio)
SELECT 
    1 AS empresa_id,
    'OT' AS tipo,
    MAX(CAST(folio AS UNSIGNED)) AS ultimo_folio
FROM ordentrabajo
WHERE empresa = 1;

INSERT INTO folios_empresa (empresa_id, tipo, ultimo_folio)
VALUES (2, 'OT', 0);

-- Datos para Factura
-- 'N00050293' o el que sea el ultimo folio
insert into folios_empresa(empresa_id, tipo, ultimo_folio)
VALUES(1, 'FA', 50293);
insert into folios_empresa(empresa_id, tipo, ultimo_folio)
VALUES(2, 'FA', 0);

-- Datos para Guia de Despacho
insert into folios_empresa(empresa_id, tipo, ultimo_folio)
VALUES(1, 'GD', 49424);
insert into folios_empresa(empresa_id, tipo, ultimo_folio)
VALUES(2, 'GD', 0);

-- Datos para Nota Credito
insert into folios_empresa(empresa_id, tipo, ultimo_folio)
VALUES(1, 'NC', 1182);
insert into folios_empresa(empresa_id, tipo, ultimo_folio)
VALUES(2, 'NC', 0);

-- Procedimiento para OT
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

-- Procedimiento para Factura
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

-- Procedimiento para GD
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

-- Procedimiento para NC

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

-- Extra -- Procedimiento para agregar una empresa e inicializar todos estos campos

DELIMITER $$

CREATE PROCEDURE crear_empresa(
    IN p_empresa_nombre VARCHAR(255))
BEGIN
	DECLARE V_EMPRESA_ID BIGINT;
    INSERT INTO empresa(nombre) VALUES (p_empresa_nombre);
    
    -- OBTENEMOS EL ID DE LA EMPRESA AÑADIDA
	SET V_EMPRESA_ID = LAST_INSERT_ID();
    
    -- INICIALIZAMOS LOS FOLIOS
	INSERT INTO folios_empresa (empresa_id, tipo, ultimo_folio)
	VALUES
		(V_EMPRESA_ID, 'OT', 0),
		(V_EMPRESA_ID, 'FA', 0),
		(V_EMPRESA_ID, 'GD', 0)
        (V_EMPRESA_ID, 'NC', 0)
	ON DUPLICATE KEY UPDATE
		ultimo_folio = ultimo_folio;
        
	-- Creamos el usuario administrador por defecto
	INSERT INTO usuario(nombre, clave, tipo, descripcion, fecha, empresa)
	VALUES ('SYS', 'SYS', 1, 'CLAVE DE ACCESO INICIAL', NOW(), V_EMPRESA_ID);
END$$

DELIMITER ;
