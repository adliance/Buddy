import {trace, context, SpanStatusCode} from '@opentelemetry/api';
import {WebTracerProvider} from '@opentelemetry/sdk-trace-web';
import {OTLPTraceExporter} from '@opentelemetry/exporter-trace-otlp-http';
import {BatchSpanProcessor, SimpleSpanProcessor, ConsoleSpanExporter} from '@opentelemetry/sdk-trace-base';
import {registerInstrumentations} from '@opentelemetry/instrumentation';
import {ZoneContextManager} from '@opentelemetry/context-zone';
import {Resource} from '@opentelemetry/resources';
import {DocumentLoadInstrumentation} from '@opentelemetry/instrumentation-document-load';


export class Telemetry {
    constructor(serviceName, exporterEndpoint = "http://localhost:4318/v1/traces") {
        // this.serviceName = serviceName;
        // this.exporterEndpoint = exporterEndpoint;

        this.exporter = new OTLPTraceExporter({
            url: exporterEndpoint
        });
        
        this.provider = new WebTracerProvider({resource: new Resource({"service.name": serviceName})});
        this.webTracerWithZone = this.provider.getTracer(serviceName);
        this.bindingSpan = undefined;

        window.startBindingSpan = (
            traceId,
            spanId,
            traceFlags
        ) => {
            this.bindingSpan = this.webTracerWithZone.startSpan("");
            this.bindingSpan.spanContext().traceId = traceId;
            this.bindingSpan.spanContext().spanId = spanId;
            this.bindingSpan.spanContext().traceFlags = traceFlags;
        };

        this.provider.addSpanProcessor(new BatchSpanProcessor(this.exporter));
        this.provider.addSpanProcessor(new SimpleSpanProcessor(new ConsoleSpanExporter()));
        this.provider.register({
            contextManager: new ZoneContextManager()
        });

        registerInstrumentations({
            instrumentations: [
                new DocumentLoadInstrumentation(),
            ],
        });
    }

    traceSpan(name, func) {
        let singleSpan;
        if (bindingSpan) {
            const ctx = trace.setSpan(context.active(), bindingSpan);
            singleSpan = this.webTracerWithZone.startSpan(name, undefined, ctx);
            this.bindingSpan = undefined;
        } else {
            singleSpan = webTracerWithZone.startSpan(name);
        }
        return context.with(trace.setSpan(context.active(), singleSpan), () => {
            try {
                const result = func();
                singleSpan.end();
                return result;
            } catch (error) {
                singleSpan.setStatus({code: SpanStatusCode.ERROR});
                singleSpan.end();
                throw error;
            }
        });
    }
}
