scaffoldr
=========

Static site generator inspired by Jekyll written in C#.

Allows multiple HTML, JPG, PNG, CSV, JSON and YAML files to construct a page, with a unique set of naming conventions.

App_Data/{kind}/{slug}/{sectionName}-{sortOrder}.{format} eg.

- App_Data/page/dataset.csv
- App_Data/page/index/metadata.yaml
- App_Data/page/index/body.html
- App_Data/page/index/thumbnail.jpg
- App_Data/page/index/cover.jpg
- App_Data/page/index/carousel-1.jpg
- App_Data/page/index/carousel-2.jpg

Plug in any template language, with a sample using Mustache templates.
