﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Wikify.Common.Content.Parsed;

namespace Wikify.Builder
{
    public class Composition : IComposition
    {
        // This implementation is thread-unsafe for performance. Instantiate as scoped.
        private ILogger _logger;
        private Stack<IState> _stateStack;
        private IWikiComponent _rootComponent;
        private IState _currentState => _stateStack.Peek();

        public Composition(ILogger<Composition> logger, IWikiComponent rootComponent, IState state)
        {
            _logger = logger;
            _rootComponent = rootComponent;

            _stateStack = new Stack<IState>();
            _stateStack.Push(state);
        }

        public async Task<IState> ApplyToolAsync(ITool tool)
        {
            var newState = await tool.ApplyAsync(_currentState);
            _stateStack.Push(newState);
            return newState;
        }
        public async Task<IState> UndoToolAsync(ITool tool)
        {
            if (_currentState.IsDefault)
            {
                var errorMessage = "Can't undo default state!";
                _logger.LogError(errorMessage);
                throw new ApplicationException(errorMessage);
            }

            // Store popped tools in reverse.
            Stack<ITool> removedTools = new();

            // Dig into the stack and find the tool to undo.
            while (true)
            {
                var state = _stateStack.Pop();

                if (state.IsDefault)
                {
                    var errorMessage = $"Reached bottom of the stack while looking for the {nameof(ITool)} to undo.";
                    _logger.LogError(errorMessage);
                    throw new ApplicationException(errorMessage);
                }

                // This is the tool to undo.
                if (state.LastToolApplied.Equals(tool))
                {
                    // The state has been popped and not added to the tools to re-apply.
                    // No action needed.
                    break;
                }

                // This wasn't the tool to undo. We will be re-applying it later.
                else
                {
                    removedTools.Push(state.LastToolApplied);
                }
            }

            // Re-apply tools above the removed one.
            for (int i = 0; i < _stateStack.Count; i++)
            {
                await ApplyToolAsync(removedTools.Pop());
            }

            return _currentState;
        }
        public async Task<IWikiComponent> BuildAsync()
        {



        }

        public async Task<IState> UndoAsync()
        {
            // Composition in default state.
            if (_stateStack.Count == 1)
            {
                if (!_currentState.IsDefault)
                {
                    var errorMessage = "The original default state has to sit at the bottom of the stack all the time.";
                    _logger.LogError(errorMessage);
                    throw new ApplicationException(errorMessage);
                }
                else
                {
                    return _currentState;
                }
            }

            else
            {
                _stateStack.Pop();
                return _stateStack.Peek();
            }

        }
    }
}