ALTER TABLE banco
ADD CONSTRAINT uq_banco_codigo_empresa
UNIQUE (codigo, empresa);

ALTER TABLE cliente
ADD CONSTRAINT uq_cliente_rut_empresa
UNIQUE (rut, empresa);