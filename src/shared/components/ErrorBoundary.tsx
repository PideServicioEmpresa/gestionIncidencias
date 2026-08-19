import { Component, type ReactNode } from 'react'

interface Props {
  children: ReactNode
  fallback?: ReactNode
  /**
   * Cuando este valor cambia, el boundary se limpia solo. Se usa con el pathname
   * para que un error en una pantalla no bloquee la navegación a las demás.
   */
  resetKey?: string | number
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

  componentDidUpdate(prevProps: Props) {
    // Al cambiar de ruta se descarta el error anterior: la pantalla nueva debe
    // intentar renderizarse en lugar de heredar el fallo de la anterior.
    if (this.state.hasError && prevProps.resetKey !== this.props.resetKey) {
      this.setState({ hasError: false, message: '' })
    }
  }

  private reintentar = () => this.setState({ hasError: false, message: '' })

  render() {
    if (this.state.hasError) {
      return (
        this.props.fallback ?? (
          <div className="flex min-h-[200px] flex-col items-center justify-center gap-2 p-6 text-center">
            <p className="text-sm font-medium text-destructive">Algo salió mal</p>
            <p className="text-xs text-muted-foreground">{this.state.message}</p>
            <p className="mt-1 max-w-xs text-xs text-muted-foreground">
              Puedes reintentar o ir a otra sección desde el menú.
            </p>
            <div className="mt-2 flex flex-wrap items-center justify-center gap-2">
              <button
                className="rounded-md border border-border px-3 py-1.5 text-xs hover:bg-muted"
                onClick={this.reintentar}
              >
                Reintentar
              </button>
              <button
                className="rounded-md border border-border px-3 py-1.5 text-xs hover:bg-muted"
                onClick={() => window.location.reload()}
              >
                Recargar la página
              </button>
            </div>
          </div>
        )
      )
    }
    return this.props.children
  }
}
