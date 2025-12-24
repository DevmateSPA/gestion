ALTER TABLE banco
ADD CONSTRAINT uq_banco_codigo_empresa
UNIQUE (codigo, empresa);

ALTER TABLE cliente
ADD CONSTRAINT uq_cliente_rut_empresa
UNIQUE (rut, empresa);

ALTER TABLE documentonulo
ADD CONSTRAINT uq_documentonulo_folio_empresa
UNIQUE (folio, empresa);

ALTER TABLE encuadernacion
ADD CONSTRAINT uq_encuadernacion_codigo_empresa
UNIQUE (codigo, empresa);

ALTER TABLE facturacompra
ADD CONSTRAINT uq_facturacompra_folio_empresa
UNIQUE (folio, empresa);

ALTER TABLE factura
ADD CONSTRAINT uq_factura_folio_empresa
UNIQUE (folio, empresa);

ALTER TABLE fotomecanica
ADD CONSTRAINT uq_fotomecanica_codigo_empresa
UNIQUE (codigo, empresa);

ALTER TABLE grupo
ADD CONSTRAINT uq_grupo_codigo_empresa
UNIQUE (codigo, empresa);

ALTER TABLE guiadespacho
ADD CONSTRAINT uq_guiadespacho_folio_empresa
UNIQUE (folio, empresa);

ALTER TABLE impresion
ADD CONSTRAINT uq_impresion_codigo_empresa
UNIQUE (codigo, empresa);

ALTER TABLE maquina
ADD CONSTRAINT uq_maquina_codigo_empresa
UNIQUE (codigo, empresa);

ALTER TABLE notacredito
ADD CONSTRAINT uq_notacredito_folio_empresa
UNIQUE (folio, empresa);

ALTER TABLE operario
ADD CONSTRAINT uq_operario_codigo_empresa
UNIQUE (codigo, empresa);

ALTER TABLE ordentrabajo
ADD CONSTRAINT uq_ordentrabajo_folio_empresa
UNIQUE (folio, empresa);

ALTER TABLE producto
ADD CONSTRAINT uq_producto_codigo_empresa
UNIQUE (codigo, empresa);

ALTER TABLE proveedor
ADD CONSTRAINT uq_proveedor_rut_empresa
UNIQUE (rut, empresa);
