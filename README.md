# ErgoSent – Sistema de Monitoreo Ergonómico en Tiempo Real

> **Un sistema inteligente para prevenir trastornos musculoesqueléticos mediante el monitoreo de postura de manos y muñecas**

---

## Descripción General

**ErgoSent** es un sistema de monitoreo ergonómico en tiempo real diseñado para rastrear la postura de manos y muñecas, previniendo trastornos musculoesqueléticos en ambientes de oficina e industria. Desarrollado en **Unity 2019.3** e integrado con la tecnología **Leap Motion**, el sistema captura datos esqueletales de la mano y los analiza contra estándares de riesgo ergonómico, proporcionando retroalimentación inmediata.

Este repositorio constituye un paquete integral de entrega que incluye el código fuente de Unity, instaladores compilados, y documentación técnica y académica extensiva.

### Contexto Institucional

ErgoSent fue desarrollado en colaboración con **COLCIENCIAS** y la **Universidad Distrital Francisco José de Caldas**, abordando los riesgos para la salud asociados con movimientos repetitivos de mano y mala postura en entornos laborales. El sistema integra marcos de referencia ergonómicos como **RULA/REBA** para la evaluación del riesgo postural.

---

## Arquitectura de Alto Nivel

El sistema actúa como un puente entre movimientos físicos de la mano y un espacio de trabajo virtual en Unity, donde se aplica la lógica ergonómica.

```
Leap Motion Sensor
       ↓
Captura de datos esquelatales (manos, dedos, huesos)
       ↓
LeapCSharp Plugin (interfaz C#)
       ↓
Análisis ergonómico en Unity
       ↓
Evaluación RULA/REBA
       ↓
Detección de incidentes y retroalimentación
       ↓
Almacenamiento en SQLite (usuario, duración, puntuación, mano)
```

### Componentes Principales

1. **Hand Tracking (Leap Motion)**
   - Utiliza el plugin administrado `LeapCSharp` para interfacear con el controlador
   - Transforma datos del sensor en objetos `Hand`, `Finger` y `Bone` dentro de Unity
   - Captura movimientos en tiempo real con precisión submilimétrica

2. **Lógica de Aplicación Unity**
   - Administrada por scripts C# personalizados (`BTNScriptsLogin`, `CSVManager`, etc.)
   - Gestiona autenticación de usuarios, transiciones entre escenas
   - Detección de incidentes ergonómicos
   - Flujo: Login → MainMenuES → ErgoSentMain

3. **Gestión de Datos**
   - Base de datos local **SQLite** para persistencia
   - Almacena información de usuarios e "Incidentes" ergonómicos
   - Registra duración, puntuación y mano involucrada
   - Exportación de datos para análisis posterior

---

## Estructura del Repositorio

El repositorio está organizado para servir tanto a usuarios finales como a desarrolladores, cubriendo todo el ciclo de vida del software.

| Artefacto | Descripción | Audiencia |
|-----------|-------------|-----------|
| `1. Ficha de Catalogación` | Metadatos según estándares COLCIENCIAS/UD | Institucional |
| `2. Manual de Usuario` | Guía operativa del software | Usuarios Finales |
| `3. Manual Desarrollador` | Guía técnica para el proyecto Unity | Desarrolladores |
| `4. Manual de Instalación` | Requisitos de setup y hardware | IT/Administración |
| `5. ErgoSent.zip` | Código fuente completo y assets de Unity | Desarrolladores |
| `6. Instalador ErgoSent.exe` | Instalador compilado para Windows | Usuarios Finales |
| `7. Registro DNA` | Registro de propiedad del software | Legal/Administración |

---

## Instalación y Requisitos

### Requisitos de Hardware

- **Leap Motion Controller** (conectado vía USB)
- Espacio mínimo: ~60 cm de distancia de captura
- Computadora con capacidades gráficas básicas de 3D

### Requisitos de Software

- **Windows 7 / 10 / 11**
- **.NET Framework 4.5** o superior
- **Leap Motion SDK** (incluido/compatible)
- **Unity 2019.3** (para desarrollo)

### Pasos de Instalación

1. Ejecuta `Instalador ErgoSent.exe`
2. Sigue el asistente de instalación
3. Conecta el dispositivo **Leap Motion**
4. Inicia la aplicación desde el menú de inicio
5. Ingresa credenciales de usuario en la pantalla de login

Para obtener detalles completos, consulta el archivo **Manual de Instalación**.

---

## Arquitectura Técnica de Unity

### Estructura de Escenas

**ErgoSentMain** es la escena principal que orquesta:

- **Captura de datos**: recibe continuamente datos esqueletales del Leap Motion
- **Análisis de postura**: evalúa posición contra umbrales ergonómicos
- **Visualización 3D**: renderiza manos virtuales y retroalimentación visual
- **Registro de incidentes**: almacena eventos de riesgo en base de datos

### Integración del Leap Motion SDK

El plugin **LeapCSharp** actúa como intermediario:

```csharp
// Ejemplo conceptual
LeapProvider leapProvider = GetComponent<LeapProvider>();
Frame frame = leapProvider.CurrentFrame;

foreach (Hand hand in frame.Hands) {
    // Análisis ergonómico
    float riskScore = CalculateRULAScore(hand);
    
    if (riskScore > THRESHOLD) {
        LogIncident(hand, riskScore);
    }
}
```

### Detección de Gestos

El sistema incluye un mecanismo de **Attachment System** para reconocer y clasificar gestos de alto riesgo, integrando análisis de:

- Ángulos articulares
- Velocidad de movimiento
- Duración de exposición
- Patrones de repetición

---

## Audiencias y Documentación

### Para Usuarios Finales

Consulta **Manual de Usuario** para:
- Guías paso a paso de operación
- Interpretación de scores y retroalimentación visual
- Mejores prácticas ergonómicas

### Para Desarrolladores

Consulta **Manual del Desarrollador** para:
- Descripción técnica de scripts C#
- Estructura de componentes Unity
- Extensión y personalización del sistema
- API de integración del Leap Motion

### Para Administradores IT

Consulta **Manual de Instalación** para:
- Requisitos de sistema detallados
- Procedimientos de despliegue
- Configuración de red (si aplica)

---

## Glosario de Términos

- **RULA**: Rapid Upper Limb Assessment; evaluación rápida de extremidad superior
- **REBA**: Rapid Entire Body Assessment; evaluación rápida de cuerpo completo
- **Leap Motion**: Sensor de seguimiento de mano por infrarrojo cercano
- **Incident**: Evento de postura de alto riesgo detectado por el sistema
- **LeapCSharp**: Plugin administrado que facilita la comunicación C#/Leap SDK
- **Bone**: Segmento óseo individual capturado (falange, metacarpo, etc.)

---

## Repositorio

- **GitHub**: [AndresPescador/ErgoSent](https://github.com/AndresPescador/ErgoSent)
- **Última indexación**: 2 de abril de 2026
- **Commit**: a9b69e05

---

## Referencias Internas

Para profundizar en aspectos específicos, consulta las secciones disponibles:

- **System Purpose and Domain**: contexto ergonómico y estándares RULA/REBA
- **Repository Structure and Distribution Artifacts**: desglose detallado de archivos y paquetes
- **Unity Application Architecture**: detalles técnicos del motor y scripts
- **Leap Motion SDK Integration**: guía de integración del sensor
- **Developer Setup**: configuración del entorno de desarrollo

---

## Licencia y Propiedad Intelectual

Registrado ante **COLCIENCIAS** y la **Universidad Distrital**. Para consultas sobre licencia, contacta la institución o el desarrollador original.

---

## Contacto y Soporte

Para preguntas técnicas, reportes de bugs, o solicitudes de soporte:

- Consulta los manuales incluidos en el repositorio
- Contacta a través del repositorio de GitHub
- Dirígete al departamento de IT/Soporte de tu institución

---

## Autoría

Este programa fue desarrollado como proyecto de tesis en la **Universidad Distrital Francisco José de Caldas** por **Daniel Nieto Gómez** bajo la dirección de **Paulo Alonso Gaona**.

---

## Disclaimer

ErgoSent proporciona análisis ergonómico asistido por tecnología y no reemplaza la evaluación profesional de especialistas en ergonomía, medicina ocupacional o fisioterapia. Úsalo como herramienta de monitoreo complementaria en programas más amplios de salud ocupacional.

---

**Última actualización**: Abril de 2026  
**Versión del documento**: 1.0
