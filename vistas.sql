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
    e.nombre AS EmpresaNombre,
    c.razon_social AS razon_social
FROM ordentrabajo t
JOIN empresa e ON t.empresa = e.id
JOIN cliente c ON t.rutcliente = c.rut;