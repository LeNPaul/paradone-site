<p>Use the <strong>Text Query</strong> field to narrow down your search with specific text matching. You can include multiple words, exclude words, or use regular expressions to filter search results.</p>

<p>For example, this text query combines Boolean and regex syntax to perform a very specific search:</p>

<p style=&quot;white-space:nowrap;&quot;><strong>ADT NOT (A03 OR A11) -ZID /Sm(i|y)th(e?)/</strong></p>

<p>This search will find log entries that contain <strong>ADT</strong> but will skip those containing <strong>A03</strong>, <strong>A11</strong> or <strong>ZID</strong>.  Search results will not include patient discharges (A03), cancelled patient admits (A11), or messages that include the custom segment ZID.</p>

<p>In addition, this search will only find records containing the name &quot;Smith&quot; and its alternate spellings (<strong>Smith</strong>, <strong>Smyth</strong>, <strong>Smythe</strong>, or <strong>Smithe</strong>).</p>