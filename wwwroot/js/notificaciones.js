function normalizeImageUrl(raw) {
    if (!raw) return null;
    if (raw.startsWith('data:') || raw.startsWith('http') || raw.startsWith('/')) return raw;
    // corregir backslashes y tomar filename
    const clean = raw.replace(/\\/g, '/');
    const fileName = clean.split('/').pop();
    return '/uploads/' + fileName;
}

// uso:
// const src = normalizeImageUrl(datos.evidenciaInicio);
// img.src = src || '/img/placeholder.png';