-- Para filtrar por empresa
CREATE INDEX idx_ot_empresa ON otprueba(empresa);

-- Para el join por folio
CREATE UNIQUE INDEX idx_unique_ot_folio ON otprueba(folio);
CREATE INDEX idx_detalle_folio ON ORDENTRABAJODETALLE(folio);