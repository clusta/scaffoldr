scaffoldr
=========

Static site generator inspired by Jekyll written in C#.

Allows multiple HTML, JPG, PNG, CSV, JSON and YAML files to construct a page, with a unique set of naming conventions.

{kind}/{slug}/{sectionName}-{sortOrder}@{variant}.{dataFormat} eg.

- data/dataset.csv
- page/index/metadata.yaml
- page/index/body.html
- page/index/thumbnail.jpg
- page/index/cover.jpg
- page/index/carousel-1.jpg
- page/index/carousel-1@2x.jpg
- page/index/carousel-2.jpg
- page/index/carousel-2@1024w.jpg
- post/my-blog-post-slug/body.html

Plug in any template language, with samples using Mustache, Razor and remote templates via HTTP.

A command line tool to run batched publishing jobs.

scaffoldr-publish -b "batch.json"​

<pre><code>
[
    {
        "kind": "post",
	    "input": {
            "base_address": "C:\\InputFolder",
            "content_path": "post\\",
            "data_path": "data\\"
        },
        "output": {
            "base_address": "C:\\OutputFolder",
            "content_type": "text/html"
        },
        "template": {
            "template_path": "C:\\TemplateFolder\\post.cshtml"
        }
    },
    {
        "kind": "page",
	    "input": {
            "base_address": "C:\\InputFolder",
            "content_path": "page\\",
            "data_path": "data\\"
        },
        "output": {
            "base_address": "C:\\OutputFolder",
            "content_type": "text/html"
        },
        "template": {
            "template_path": "C:\\TemplateFolder\\page.cshtml"
        }
    }
]
</code></pre>
