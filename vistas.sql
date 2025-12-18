-- Para Factura
CREATE OR REPLACE VIEW vw_factura AS
    SELECT
        t.*,
        e.nombre AS EmpresaNombre
    FROM FACTURA t
    JOIN EMPRESA e
    ON t.empresa = e.id;

-- Para Banco
CREATE OR REPLACE VIEW vw_banco AS
    SELECT
        t.*,
        e.nombre AS EmpresaNombre
    FROM BANCO t
    JOIN EMPRESA e
    ON t.empresa = e.id;

-- Para Cliente
CREATE OR REPLACE VIEW vw_cliente AS
    SELECT
        t.*,
        e.nombre AS EmpresaNombre
    FROM CLIENTE t
    JOIN EMPRESA e
    ON t.empresa = e.id;

-- Para DocumentoNulo
CREATE OR REPLACE VIEW vw_documentonulo AS
    SELECT
        t.*,
        e.nombre AS EmpresaNombre
    FROM DOCUMENTONULO t
    JOIN EMPRESA e
    ON t.empresa = e.id;

-- Para Encuadernacion
CREATE OR REPLACE VIEW vw_encuadernacion AS
    SELECT
        t.*,
        e.nombre AS EmpresaNombre
    FROM ENCUADERNACION t
    JOIN EMPRESA e
    ON t.empresa = e.id;

-- Para FacturaCompra
CREATE OR REPLACE VIEW vw_facturacompra AS
    SELECT
        t.*,
        e.nombre AS EmpresaNombre
    FROM FACTURACOMPRA t
    JOIN EMPRESA e
    ON t.empresa = e.id;

-- Para Fotomecanica
CREATE OR REPLACE VIEW vw_fotomecanica AS
    SELECT
        t.*,
        e.nombre AS EmpresaNombre
    FROM FOTOMECANICA t
    JOIN EMPRESA e
    ON t.empresa = e.id;

-- Para Grupo
CREATE OR REPLACE VIEW vw_grupo AS
    SELECT
        t.*,
        e.nombre AS EmpresaNombre
    FROM GRUPO t
    JOIN EMPRESA e
    ON t.empresa = e.id;

-- Para GuiaDespacho
CREATE OR REPLACE VIEW vw_guiadespacho AS
    SELECT
        t.*,
        e.nombre AS EmpresaNombre
    FROM GUIADESPACHO t
    JOIN EMPRESA e
    ON t.empresa = e.id;

-- Para Impresion
CREATE OR REPLACE VIEW vw_impresion AS
    SELECT
        t.*,
        e.nombre AS EmpresaNombre
    FROM IMPRESION t
    JOIN EMPRESA e
    ON t.empresa = e.id;

-- Para Maquina
CREATE OR REPLACE VIEW vw_maquina AS
    SELECT
        t.*,
        e.nombre AS EmpresaNombre
    FROM MAQUINA t
    JOIN EMPRESA e
    ON t.empresa = e.id;

-- Para NotaCredito
CREATE OR REPLACE VIEW vw_notacredito AS
    SELECT
        t.*,
        e.nombre AS EmpresaNombre
    FROM NOTACREDITO t
    JOIN EMPRESA e
    ON t.empresa = e.id;

-- Para Operario
CREATE OR REPLACE VIEW vw_operario AS
    SELECT
        t.*,
        e.nombre AS EmpresaNombre
    FROM OPERARIO t
    JOIN EMPRESA e
    ON t.empresa = e.id;

-- Para Producto
CREATE OR REPLACE VIEW vw_producto AS
    SELECT
        t.*,
        e.nombre AS EmpresaNombre
    FROM PRODUCTO t
    JOIN EMPRESA e
    ON t.empresa = e.id;

-- Para Proveedor
CREATE OR REPLACE VIEW vw_proveedor AS
    SELECT
        t.*,
        e.nombre AS EmpresaNombre
    FROM PROVEEDOR t
    JOIN EMPRESA e
    ON t.empresa = e.id;

CREATE OR REPLACE
ALGORITHM = UNDEFINED
DEFINER = `root`@`localhost`
SQL SECURITY DEFINER
VIEW vw_ordentrabajo AS
SELECT 
    t.*,
    c.razon_social AS razon_social,
    m.descripcion AS Maquina1descripcion
FROM ordentrabajo t
JOIN empresa e ON t.empresa = e.id
LEFT JOIN cliente c ON t.rutcliente = c.rut;
LEFT JOIN maquina m ON t.maquina1 = m.codigo;

-- Vista para trabajar con las Maquinas con pendientes
CREATE OR REPLACE VIEW vw_maquinas_with_pending_orders AS 
    SELECT DISTINCT
        m.codigo,
        m.descripcion,
        COUNT(ot.maquina1) AS cantidad_pendientes
    FROM maquina m
    JOIN ordentrabajo ot
        ON m.codigo = ot.maquina1
    GROUP BY
        m.codigo,
        m.descripcion;