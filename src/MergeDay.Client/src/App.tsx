import React from "react";

const App: React.FC = () => {
  return (
    <div className="min-h-dvh flex flex-col bg-neutral-50 text-neutral-900">
      {/* HEADER */}
      <header className="h-14 border-b bg-white">
        <div className="mx-auto max-w-screen-xl h-full flex items-center justify-between px-4">
          <a className="font-semibold">MySite</a>
          <nav className="hidden md:flex gap-6">
            <a className="hover:text-blue-600">Home</a>
            <a className="hover:text-blue-600">About</a>
            <a className="hover:text-blue-600">Contact</a>
          </nav>
          <button
            className="md:hidden border rounded px-3 py-1"
            aria-label="Open menu"
          >
            ☰
          </button>
        </div>
      </header>

      {/* MAIN */}
      <main className="flex-1">
        <div className="mx-auto max-w-screen-xl px-4 py-8">
          <h1 className="text-3xl font-bold mb-4">Welcome!</h1>
          <p className="text-neutral-700">
            This is your basic layout — a header, a main content area, and a footer.
          </p>
        </div>
      </main>

      {/* FOOTER */}
      <footer className="h-16 border-t bg-white">
        <div className="mx-auto max-w-screen-xl h-full flex items-center justify-center px-4 text-sm text-neutral-500">
          © {new Date().getFullYear()} MySite
        </div>
      </footer>
    </div>
  );
};

export default App;
