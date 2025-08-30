#!/bin/sh
set -eu

# Defaults
: "${API_URL:=/api}"
: "${ENV_KEYS:=API_URL}"

CONFIG_PATH="/usr/share/nginx/html/config.js"
TMP_PATH="/tmp/config.js.$$"

# Build a JS object from the listed ENV_KEYS (comma-separated)
# Example: ENV_KEYS="API_URL,ENABLE_DEBUG,PUBLIC_SENTRY_DSN"
echo "window.RUNTIME_CONFIG = {" > "$TMP_PATH"
first=true
# shellcheck disable=SC2046
for key in $(echo "$ENV_KEYS" | tr ',' ' '); do
  val="$(printenv "$key" || true)"
  [ -z "${val:-}" ] && continue

  # Best-effort type detection for booleans/numbers; else quote as string
  case "$val" in
    true|false|TRUE|FALSE|[0-9]*)
      jsval="$val"
      ;;
    *)
      # Escape backslashes and double quotes
      esc=$(printf "%s" "$val" | sed 's/\\/\\\\/g; s/"/\\"/g')
      jsval="\"$esc\""
      ;;
  esac

  if [ "$first" = true ]; then
    first=false
  else
    printf ",\n" >> "$TMP_PATH"
  fi
  printf "  %s: %s" "$key" "$jsval" >> "$TMP_PATH"
done
printf "\n};\n" >> "$TMP_PATH"

mv "$TMP_PATH" "$CONFIG_PATH"

exec "$@"
