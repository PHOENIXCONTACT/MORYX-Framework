# MORYX Website

This branch holds the source of the MORYX project website, [www.moryx-industry.net](https://www.moryx-industry.net). It's a Jekyll site built and published by GitHub Pages.

## Preview locally

With [Ruby](https://www.ruby-lang.org/en/documentation/installation/) 3.x and Bundler installed:

```bash
bundle install
bundle exec jekyll serve
```

Then open <http://localhost:4000>. The page reloads when you change files; restart the server after editing `_config.yml`.

Or, with Docker only:

```bash
docker run --rm -it -p 4000:4000 -v "$PWD":/site -w /site ruby:3.3 \
  bash -c "bundle install && bundle exec jekyll serve --host 0.0.0.0"
```
