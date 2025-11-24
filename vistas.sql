-- Para Factura
CREATE OR REPLACE VIEW vw_factura AS
	SELECT
    	t.*,
        e.nombre as EmpresaNombre
  	FROM FACTURA t
    JOIN EMPRESA e
    ON t.empresa = e.id;