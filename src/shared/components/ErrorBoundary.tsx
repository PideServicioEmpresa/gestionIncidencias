import { Component, type ReactNode } from 'react'

interface Props {
  children: ReactNode
  fallback?: ReactNode
}

interface State {
  hasError: boolean
  message: string
}

export class ErrorBoundary extends Component<Props, State> {
  state: State = { hasError: false, message: '' }

  static getDerivedStateFromError(error: unknown): State {
    const message = error instanceof Error ? error.message : 'Error inesperado.'
    return { hasError: true, message }
  }

  render() {
    if (this.state.hasError) {
      return (
        this.props.fallback ?? (
          <div className="flex min-h-[200px] flex-col items-center justify-center gap-2 p-6 text-center">
            <p className="text-sm font-medium text-destructive">Algo salió mal</p>
            <p className="text-xs text-muted-foreground">{this.state.message}</p>
            <button
              className="mt-2 rounded-md border border-border px-3 py-1.5 text-xs hover:bg-muted"
              onClick={() => this.setState({ hasError: false, message: '' })}
            >
              Reintentar
            </button>
          </div>
        )
      )
    }
    return this.props.children
  }
}
